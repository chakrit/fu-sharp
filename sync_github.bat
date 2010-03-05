@ECHO OFF

git svn clone -s https://buildsvnwiki.myidxnet.localdomain/svn/FuSharp git_sync_temp
cd git_sync_temp
git remote add origin git@github.com:chakrit/fu-sharp.git
git push origin master

rd /s git_sync_tmep
