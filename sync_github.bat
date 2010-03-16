@ECHO OFF

IF EXIST git_sync_temp RD /s git_sync_temp

CMD /C git svn clone -s https://buildsvnwiki.myidxnet.localdomain/svn/FuSharp git_sync_temp --no-metadata
CD git_sync_temp
CMD /C git remote add origin git@github.com:chakrit/fu-sharp.git

PAUSE
CMD /C git push origin master

CD ..
RD /s git_sync_temp
