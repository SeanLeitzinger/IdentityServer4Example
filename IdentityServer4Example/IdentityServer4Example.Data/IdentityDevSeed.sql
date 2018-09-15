USE [IdServer4Example];
GO
--Clients

INSERT INTO Dbo.Clients( [Absoluterefreshtokenlifetime], [Accesstokenlifetime], [Accesstokentype], [Allowaccesstokensviabrowser], [Allowofflineaccess], [Allowplaintextpkce], [Allowrememberconsent],
[Alwaysincludeuserclaimsinidtoken], [Alwayssendclientclaims], [Authorizationcodelifetime], [BackChannelLogoutSessionRequired], [Clientid], [Clientname], [Clienturi], [Enablelocallogin], [Enabled], [FrontChannelLogoutSessionRequired], [Identitytokenlifetime],
[Includejwtid], [Logouri], [Protocoltype], [Refreshtokenexpiration], [Refreshtokenusage], [Requireclientsecret], [Requireconsent],
[Requirepkce], [Slidingrefreshtokenlifetime], [Updateaccesstokenclaimsonrefresh] )
VALUES( 
	   2592000, 3600, 0, 1, 1, 0, 1, 0, 0, 300, 0, 'examplewebclient', 'ExampleWebClient', 'https://localhost:44322', 1, 1, 0, 300, 0, NULL, 'oidc', 1, 1, 1, 0, 0, 1296000, 0 );
GO

INSERT INTO IdentityResources([Description], [DisplayName], [Emphasize], [Enabled], [Name], [Required], [ShowInDiscoveryDocument])
VALUES  ('', 'Your user identifier', 0, 1, 'openid', 1, 1),
		('Your user profile information (first name, last name, etc.)', 'User profile', 1, 1, 'profile', 0, 1),
		('', 'Your email address', 1, 1, 'email', 0, 1)
--ClientScopes

DECLARE @exampleWebClientId INT;

SELECT @exampleWebClientId = [C].[Id]
FROM Dbo.Clients AS C
WHERE [C].[Clientname] = 'ExampleWebClient';

INSERT INTO Dbo.Clientscopes( [Clientid], [Scope] )
VALUES (@exampleWebClientId, 'exampleapi' ),
	   (@exampleWebClientId, 'openid' ),
	   (@exampleWebClientId, 'profile'),
	   (@exampleWebClientId, 'email');
--ClientSecrets

INSERT INTO Dbo.Clientsecrets( [Clientid], [Description], [Expiration], [Type], [Value] )
VALUES( 
	   @exampleWebClientId, NULL, NULL, 'SharedSecret', 'K7gNU3sdo+OL0wNhqoVWhr3g6s1xYv72ol/pe/Unols=' );
--ClientRedirectUris

INSERT INTO Dbo.Clientredirecturis( [Clientid], [Redirecturi] )
VALUES( 
	   @exampleWebClientId, 'https://localhost:44322/signin-callback.html'),
	   (@exampleWebClientId, 'https://localhost:44322/silent-renew-callback.html');
--ClientPostLogoutRedirectUris

INSERT INTO Dbo.Clientpostlogoutredirecturis( [Clientid], [Postlogoutredirecturi] )
VALUES( 
	   @exampleWebClientId, 'https://localhost:44322/signout-callback-oidc' );
--ClientGrantTypes

INSERT INTO Dbo.Clientgranttypes( [Clientid], [Granttype] )
VALUES( 
	   @exampleWebClientId, 'implicit' );
--ClientCorsOrigins

INSERT INTO Dbo.Clientcorsorigins( [Clientid], [Origin] )
VALUES(@exampleWebClientId, 'https://localhost:44322');
--ApiResources

INSERT INTO Dbo.Apiresources( [Description], [Displayname], [Enabled], [Name] )
VALUES( 
	   NULL, 'Example Web API', 1, 'exampleapi' );

DECLARE @exampleApiId INT;

SELECT @exampleApiId = [A].[Id]
FROM Dbo.Apiresources AS A
WHERE [A].[Name] = 'exampleapi';
--ApiScopes

INSERT INTO Dbo.Apiscopes( [Apiresourceid], [Description], [Displayname], [Emphasize], [Name], [Required], [Showindiscoverydocument] )
VALUES( 
	   @exampleApiId, NULL, 'Example Web API', 0, 'exampleapi', 0, 0 );
GO
