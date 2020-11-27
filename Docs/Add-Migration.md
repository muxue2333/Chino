
Chino 

``` powershell
Add-Migration InitChinoApp -Context ChinoApplicationDbContext -OutputDir Data\Migrations\Chino\Application
```

Identity Server

``` powershell
Add-Migration InitConfigurations -Context ConfigurationDbContext -OutputDir Data\Migrations\IdentityServer\Configuration

Add-Migration InitPersisted -Context PersistedGrantDbContext -OutputDir Data\Migrations\IdentityServer\PersistedGrant
```