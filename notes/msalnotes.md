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