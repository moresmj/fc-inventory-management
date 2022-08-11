SET rootpath=%cd%
SET dotnetSource=%rootpath%\API\FloorCenter\FC.Api\
:: dotnet build %dotnetSource%FC.Api.csproj /clp:ErrorsOnly /p:DeployOnBuild=true /p:DesktopBuildPackageLocation="obj/%Configuration%/netcoreapp2.0/package.zip" /p:WebPublishMethod=Package /p:Configuration=%Configuration% /p:IncludeSetAclProviderOnDestination=False
dotnet build %dotnetSource%FC.Api.csproj /clp:ErrorsOnly /p:DeployOnBuild=true /p:DeployTarget=Package /p:Configuration=%Configuration% /p:IncludeSetAclProviderOnDestination=False

:: app_offline.htm make site temporary offline for deployment
"%msdeployPath%msdeploy.exe" -verb:sync -allowUntrusted:true -source:contentPath=%rootpath%\API\FloorCenter\AppOffline\app_offline.htm -dest:contentPath=%IISAppPath%\app_offline.htm,ComputerName=%CName%,UserName=%User%,Password=%Password%,AuthType=Basic

:: Deploy source to site destination
"%msdeployPath%msdeploy.exe" -verb:sync -allowUntrusted:true -enableRule:DoNotDeleteRule -source:contentPath=%dotnetSource%obj\%Configuration%\netcoreapp2.0\PubTmp\Out -dest:contentPath=%IisAppPath%,ComputerName=%CName%,UserName=%User%,Password=%Password%,AuthType=Basic,includeAcls=False

:: deleting app_offline.htm
"%msdeployPath%msdeploy.exe" -verb:delete -allowUntrusted:true -dest:contentPath=%IISAppPath%\app_offline.htm,ComputerName=%CName%,UserName=%User%,Password=%Password%,AuthType=Basic