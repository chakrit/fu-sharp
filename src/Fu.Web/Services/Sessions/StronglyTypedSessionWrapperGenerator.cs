
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Microsoft.CSharp;

namespace Fu.Services.Sessions
{
  public class StronglyTypedSessionWrapperGenerationException<T> : Exception
      where T : class
  {
    public StronglyTypedSessionWrapperGenerationException() :
      base("Unable to generate wrapper for type " + typeof(T).Name +
        ". It must be an interface and only contains properties.") { }
  }

  public class StronglyTypedSessionWrapperGenerator
  {
    public Func<ISession, T> Generate<T>() where T : class
    {
      var provider = new CSharpCodeProvider();

      var unit = BuildWrapperType<T>();

      var targetAsmFilename =
          Path.GetFileName(typeof(T).Assembly.CodeBase);

      unit.ReferencedAssemblies.Add("System.dll");
      unit.ReferencedAssemblies.Add("fu.dll");
      unit.ReferencedAssemblies.Add("fu.web.dll");
      unit.ReferencedAssemblies.Add(targetAsmFilename);

      CompilerResults result;
      // TempFileCollection tfc = new TempFileCollection(
      //   Environment.CurrentDirectory, true);

      // result = new CompilerResults(tfc);
      result = provider.CompileAssemblyFromDom(new CompilerParameters {
        GenerateExecutable = false,
        GenerateInMemory = false,
        OutputAssembly = "fu.generated.dll",
        IncludeDebugInformation = false,
      }, unit);

      /* BEGIN DEBUG
      provider.GenerateCodeFromCompileUnit(unit, Console.Out,
          new CodeGeneratorOptions
          {
              BracingStyle = "C",
              IndentString = "  ",
              BlankLinesBetweenMembers = false
          });

      foreach (CompilerError err in result.Errors)
          Console.WriteLine(err.ToString());
      // END DEBUG */

      var asm = Assembly.LoadFrom(result.PathToAssembly);

      var interfaceType = typeof(T);
      var type = asm
        .GetTypes()
        .First(t => interfaceType.IsAssignableFrom(t));

      // compile a lambda for fastest result
      return BuildLambdaActivator<T>(type);
    }

    public Func<ISession, T> BuildLambdaActivator<T>(Type wrapperType) where T : class
    {
      var sessionType = typeof(ISession);
      var param = Expression.Parameter(sessionType, "session");

      var ctor = wrapperType.GetConstructor(new[] { sessionType });
      var activation = Expression.New(ctor, param);

      var lambda = Expression.Lambda(typeof(Func<ISession, T>),
          activation, param);

      return (Func<ISession, T>)lambda.Compile();
    }

    public CodeCompileUnit BuildWrapperType<T>()
        where T : class
    {
      var userType = typeof(T);
      var userTypeRef = new CodeTypeReference(userType);

      if (!userType.IsInterface)
        throw new StronglyTypedSessionWrapperGenerationException<T>();

      var sessionType = typeof(ISession);
      var sessionTypeRef = new CodeTypeReference(sessionType);

      var backingFieldName = "_session";
      var ctorParamName = "session";

      // generate the wrapper shell
      // public class GeneratedStronglyTypedIUserSession { }
      var wrapper = new CodeTypeDeclaration("GeneratedStronglyTyped" + userType.Name) {
        IsClass = true,
        Attributes = MemberAttributes.Public
      };

      wrapper.BaseTypes.Add(userTypeRef);

      // add a backing field to store the ISession reference
      // private ISession _session;
      var backingField = new CodeMemberField(sessionTypeRef, backingFieldName) {
        Attributes = MemberAttributes.Private
      };

      wrapper.Members.Add(backingField);

      // make a reference to this field
      var backingFieldRef = new CodeFieldReferenceExpression(
        new CodeThisReferenceExpression(), backingFieldName);

      // and a constructor to accept an ISession reference and save it
      // public Class(ISession session) { }
      var ctor = new CodeConstructor();
      ctor.Attributes = MemberAttributes.Public;
      ctor.Parameters.Add(
        new CodeParameterDeclarationExpression(sessionType, ctorParamName));

      // this._session = session;
      var right = new CodeVariableReferenceExpression(ctorParamName);
      var assignment = new CodeAssignStatement(backingFieldRef, right);

      ctor.Statements.Add(assignment);
      wrapper.Members.Add(ctor);

      // add support for a "Destroy" method, if the interface has it.
      var destroyMethod = userType.GetMethod("Destroy");
      if (destroyMethod != null) {

        var wDestroy = new CodeMemberMethod {
          Attributes = MemberAttributes.Public,
          ReturnType = new CodeTypeReference(destroyMethod.ReturnType),
          Name = destroyMethod.Name,
        };

        // this._session.Destroy();
        var destroyMethodRef = new CodeMethodReferenceExpression(
          backingFieldRef, "Destroy");
        var destroyInvocation = new CodeMethodInvokeExpression(destroyMethodRef);

        wDestroy.Statements.Add(destroyInvocation);
        wrapper.Members.Add(wDestroy);
      }

      // add getter/setters
      var interfaceProperties = userType.GetProperties();
      foreach (var prop in interfaceProperties) {
        // public PropertyType Name { get { ? } set { ? } }
        var wProp = new CodeMemberProperty {
          Attributes = MemberAttributes.Public,
          Type = new CodeTypeReference(prop.PropertyType),
          Name = prop.Name,
        };

        if (prop.CanRead) {
          wProp.HasGet = true;

          // return this._session.Get<PropertyType>("Name");
          var getMethod = new CodeMethodReferenceExpression(
              backingFieldRef, "Get", wProp.Type);
          var getInvocation = new CodeMethodInvokeExpression(
              getMethod, new CodePrimitiveExpression(wProp.Name));
          var returnGet = new CodeMethodReturnStatement(getInvocation);

          wProp.GetStatements.Add(returnGet);
        }

        if (prop.CanWrite) {
          wProp.HasSet = true;

          // this._session.Set("Name", value);
          var setMethod = new CodeMethodReferenceExpression(
              backingFieldRef, "Set");
          var setInvocation = new CodeMethodInvokeExpression(setMethod,
              new CodePrimitiveExpression(wProp.Name),
              new CodePropertySetValueReferenceExpression());

          wProp.SetStatements.Add(setInvocation);
        }

        wrapper.Members.Add(wProp);
      }


      // put it in a namespace and its own compile unit
      var ns = new CodeNamespace("Fu.Generated");
      ns.Types.Add(wrapper);

      var unit = new CodeCompileUnit();
      unit.Namespaces.Add(ns);

      return unit;
    }
  }
}
