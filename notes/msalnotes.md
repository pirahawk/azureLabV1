# Links
* https://docs.microsoft.com/en-us/azure/active-directory/develop/reference-v2-libraries
* Azure AD built in Roles (diff from normal AZ PASS roles) https://docs.microsoft.com/en-us/azure/active-directory/roles/permissions-reference


# Some knowhow behind setting up the AD tenant
Started here
https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-create-new-tenant

This informs either creating an AD or an ADB2C tenant.



## Azure AD specific stuff
If I wanted to create a full fledged Azure AD - these links are interesting
AD fundamentals page: https://docs.microsoft.com/en-us/azure/active-directory/fundamentals/active-directory-access-create-new-tenant

creating AD users
https://docs.microsoft.com/en-us/azure/active-directory/fundamentals/add-users-azure-active-directory





## Azure AD B2C - Tutorial to create a Tenant and other important links

However I already have an AD B2C tenant so went to 
https://docs.microsoft.com/en-us/azure/active-directory-b2c/tutorial-create-tenant   --> **This is an important starting point**

Following through the steps 1-5 here revealed `vital` information. 
**Importantly, step 2 - Register Web Application: "Enable ID token implicit grant"** revealed some important insight into the different types of Token grant flows (see section below with more links). For this tutorial, we used the `Implicit Token Grant flow`.


Other Notes:
Technical Overview. **There is some good info on the ADB2C account types here, particularly Consumer accounts** https://docs.microsoft.com/en-us/azure/active-directory-b2c/technical-overview

There are also explicit code examples for various application situations listed in the docs:
**I wanted to know about "Configure authentication in a sample web app by using Azure AD B2C"** https://docs.microsoft.com/en-us/azure/active-directory-b2c/configure-authentication-sample-web-app?tabs=visual-studio 
leads to this git code with examples https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2/tree/master/1-WebApp-OIDC/1-5-B2C  

There are also **many** other application based examples listed under that section like "Configure authentication in a sample web app that calls a web API by using Azure AD B2C" which may also be useful.




## About the Implicit Token Grant flow - (Diff types of Token Grant Flows)

From looking at the "Tutorial to create a Tenant" section, I noticed that all this while, we have been using the OAuth 2.0 `implicit grant flow` (a type of Token Grant Flow), what this means is that when the `/authorize` endpoint is called, it returns both the ID and/or the access tokens (depending on what you configure as per this https://docs.microsoft.com/en-us/azure/active-directory-b2c/tutorial-register-applications?tabs=app-reg-ga#enable-id-token-implicit-grant ) via the `/authorize` endpoint. When getting both the ID + Auth token, its known as a "hybrid flow". When its just getting the Auth token, it seems to be referred to as the "Implicit Flow". (see links)

See this link for better explanation
https://docs.microsoft.com/en-gb/azure/active-directory/develop/v2-oauth2-implicit-grant-flow?WT.mc_id=Portal-Microsoft_AAD_RegisteredApps



This "hybrid" implicit flow is different instead of an OAuth 2.0 `authorzation-code-flow` (another type of Token Grant Flow) which prefers calling to `/authorize` first and then calling to the `/token` endpoint to retrieve the Id/access token. **MS DOCS dictate that the Auth code flow should be preferred to the Hybrid implcit grant flow**
https://docs.microsoft.com/en-gb/azure/active-directory/develop/v2-oauth2-auth-code-flow


**SEE ALL THE VARIOUS TOKEN GRANT FLOWS DESCRIBED HERE UNDER THE MS DOCS IDENTITY PLATFORM DOCUMENTATION** https://docs.microsoft.com/en-gb/azure/active-directory/develop/v2-oauth2-auth-code-flow




# MSAL Github project

The root MSAL github project is here
https://github.com/AzureAD/microsoft-identity-web


Sample code (referenced from MS docs) leads to this repo
https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2  **THESE ARE IMPORTANT AND VALID EXAMPLES AND MORE MODERN UPTO DATE CODE SAMPLES(even more modern than the ones seen in section for Tutorial to create a Tenant)**



Notice that for example this location in the Repo
https://github.com/AzureAD/microsoft-identity-web/blob/master/src/Microsoft.Identity.Web.UI/Areas/MicrosoftIdentity/Controllers/AccountController.cs

This is like where the Code samples, like at https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2/blob/master/1-WebApp-OIDC/1-5-B2C/Views/Shared/_LoginPartial.cshtml
have the blocks to call down to the MSAL bits from the views like so (see the area and controller)
```
<ul class="nav navbar-nav navbar-right">
        <li class="navbar-btn">
            <form method="get" asp-area="MicrosoftIdentity" asp-controller="Account" asp-action="SignIn">
                <button type="submit" class="btn btn-primary">Sign Up/In</button>
            </form>
        </li>
    </ul>
```

In the Past you have tried to do all this yourself, maybe don't and let MSAL do that work for you. but its good to get an idea of how the code is laid out



# Which Nuget package to use
See this on the MSAL site about Microsoft identity platform authentication libraries
https://docs.microsoft.com/en-us/azure/active-directory/develop/reference-v2-libraries


https://docs.microsoft.com/en-us/azure/active-directory/develop/web-app-quickstart?pivots=devlang-aspnet-core


https://docs.microsoft.com/en-us/azure/active-directory-b2c/configure-authentication-sample-web-app?tabs=visual-studio




# Ex 1 - Simple Auth using Hybrid workflow

In this solution `https://github.com/pirahawk/azureLabV1/commit/9ae25a4e15fe5991cebc463e9fdb0c9e6ffc005d` what I had was a simple UI Web app using MSAL to auth with ADB2C.

I was using `Microsoft.Identity.Web` and the `Microsoft.Identity.Web.UI` package to integrate with MSAL

To configure MSAL, these were my user secrets:
```
 ❯ dotnet user-secrets list

AzureAdB2C:SignUpSignInPolicyId = B2C_1_azurelab_v1_web_si
AzureAdB2C:SignedOutCallbackPath = /auth-logout/B2C_1_azurelab_v1_web_si
AzureAdB2C:Instance = https://<hidden>.b2clogin.com
AzureAdB2C:Domain = <hidden>.onmicrosoft.com
AzureAdB2C:ClientId = 882e8<HIDDEN>
AzureAdB2C:CallbackPath = /auth-login
```

Things i needed to do to get this working:

1) Important to note that I needed the `AzureAdB2C:CallbackPath` setting to match the same route I had set on the Application registration in the AZ B2C tenant for the `Web Redirect URL`. Without this, the MSAL library defaults to a some preconfigured route. You then get an error because the login route does not match what the application was expecting.


2) I used an ASP razor PageModel to setup the Login route like so. Note that you need to make it `IgnoreAntiforgeryToken` otherwise it always expects that and that causes a failure as well. (there must be a way to get the CSRF token to be returned from AD B2C just didn't bother atm)

```
    [AllowAnonymous]
    [IgnoreAntiforgeryToken] // You need to set this or get 400 all the time because of CSRF token missing
    public class AuthLoginModel : PageModel
    {
        public void OnGet()
        {
        }

        
        public void OnPost()
        {
            var user = this.User;
        }
    }
```

Things observed during developing and running this solution:

1) When the app runs and I hit the `Sign-in` buttong, I get redirected to the `https://<hidden>.b2clogin.com/<hidden>.onmicrosoft.com/b2c_1_azurelab_v1_web_si/oauth2/v2.0/authorize?` endpoint. This was as expected. At this point I proceed to do the authentication for the test account on the ADB2C site. This request has a number of URL params in it:
```
client_id
	882e8268-<hidden>
redirect_uri
	https://localhost:7195/auth-login           // Imp as this is what is obtained from the MSAL configuration
response_type
	id_token
scope
	openid profile
response_mode
	form_post
nonce
	637991095083790643.YTM2NTczODgtMjhlNy00ZDM1LTkxMGUtY....<hidden>       // This is reflected in the Auth JWT token claims when recieved back. Note that the second part split after the '.' character is base64 encoded
client_info
	1
x-client-brkrver
	IDWeb.1.25.2.0
state
	CfDJ8OWWyQWR8SRMnaCHCUxpRmRhcOfpL4B1qq-.....<hidden>
```

2) When done, ADB2C `POST` requests back to the apps `/auth-login` endpoint. This post request had the following payload

```
state=CfDJ8OWWyQWR8SRMnaCHCUxpRmRhcOfpL4B1qq-.....<hidden>           // This is reflected from what is passed in the URL params on the call to the ADB2C /auth endpoint
&client_info=eyJ1a.....<hidden>   // This is a base64 string
&id_token=eyJ0eX.....<hidden>         // This is JWT Token that contains the Identity information
```

3) **Something to note** When I had initially had a mismatch where on ADB2C I had specified a `Web Redirect URI` for the APP registration but then forgot to set the `AzureAdB2C:CallbackPath`, MSAL automatically generated a path for the callback something along the lines of `redirect_uri: https://localhost:7195/signin-oidc` (in the URL params call to the ADB2C auth endpoint).

When ADB2C got the request, it immediately `POST` request back to the `Web Redirect URI` that was conifigured on the app registration (which was the intended `/auth-login`) with the following REQEST body payload

```
error=redirect_uri_mismatch
&error_description=AADB2C90006%3A+The+redirect+URI+%27https%3A%2F%2Flocalhost%3A7195%2Fsignin-oidc%27+provided+in+the+request+is+not+registered+for+the+client+id+%27882e8268-7886-491f-8da1-a03826fbd6ed%27.%0D%0ACorrelation+ID%3A+821a9e9d-486f-4769-8821-0322421e338d%0D%0ATimestamp%3A+2022-09-18+15%3A21%3A51Z%0D%0A
state=CfDJ8OWWyQWR8SRMnaCHCUxpRmTgMBD2kiCMv4B_wwrmgS-.....<hidden>  // This is reflected from what is passed in the URL params on the call to the ADB2C /auth endpoint
```

This was noticed when I looked at the `this.Request.Form["error"]` property of the ASP Razor Page when debugging the solution with a breakpoint on the POST route method in the model class.