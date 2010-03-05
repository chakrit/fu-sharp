@ECHO OFF

IF EXIST git_sync_temp rd /s git_sync_temp

git svn clone -s https://buildsvnwiki.myidxnet.localdomain/svn/FuSharp git_sync_temp --no-metadata
cd git_sync_temp
git remote add origin git@github.com:chakrit/fu-sharp.git
git push origin master

cd ..
rd /s git_sync_tmep
