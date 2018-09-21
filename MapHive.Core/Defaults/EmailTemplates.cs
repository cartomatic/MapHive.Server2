using System;
using System.Collections.Generic;
using System.Text;
using MapHive.Core.DataModel;

namespace MapHive.Core.Defaults
{
    public class EmailTemplates
    {
        public static List<EmailTemplateLocalization> GetEmailTemplates()
        {
            var activateAccountLink = MapHive.Core.Configuration.WebClientConfiguration.ActivateAccountLinkHash;
            var resetPassLink = MapHive.Core.Configuration.WebClientConfiguration.ResetPassLinkHash;

            return new List<EmailTemplateLocalization>
            {
                new EmailTemplateLocalization
                {
                    Uuid = Guid.Parse("ee624417-8565-49c8-b19a-83f3b0da8550"),
                    Name = "User created",
                    Description =
@"Email sent when user has been created.
Replacement tokens are:
    {InitialPassword},
    {VerificationKey},
    {UserName},
    {Email}
    {RedirectUrl}
",
                    Identifier = "user_created",
                    IsBodyHtml = true,
                    Translations = new EmailTranslations
                    {
                        { "en",
                            new EmailTemplate{
                                Title = "[no-reply@maphive.net] MapHive user account created for {UserName}.",
                                Body =
$@"<h3>Hi {{UserName}}</h3>
<p>MapHive user account has been created for you.</p>
<p>Your initial password is: <b>{{InitialPassword}}</b></p>
<br/>
<p>In order to activate your account please click <a href=""{activateAccountLink}"">here</a> or paste the following link in your browser: {activateAccountLink}</p>
<br/>
<p>Kind regards<br/>MapHive Team</p>"
                            }
                        }
                    }
                },

                //account activation verification key stale
                new EmailTemplateLocalization
                {
                    Uuid = Guid.Parse("e74b24a5-b541-4cb5-87d3-fb98bc83c541"),
                    Name = "Account activation verification key stale",
                    Description =
@"Email sent when account activation is not possible due to a verication key being stale.
Replacement tokens are:
    {InitialPassword},
    {VerificationKey},
    {UserName},
    {Email}
    {RedirectUrl}
",
                    Identifier = "activate_account_stale",
                    IsBodyHtml = true,
                    Translations = new EmailTranslations
                    {
                        { "en",
                            new EmailTemplate{
                                Title = "[no-reply@maphive.net] MapHive account activation required for {UserName}.",
                                Body =
$@"<h3>Hi {{UserName}}</h3>
<p>MapHive user account could not have been activated due to the verification key becoming outdated.</p>
<p>Your initial password is: <b>{{InitialPassword}}</b></p>
<br/>
<p>In order to activate your account please click <a href=""{activateAccountLink}"">here</a> or paste the following link in your browser: {activateAccountLink}</p>
<br/>
<p>Kind regards<br/>MapHive Team</p>"
                            }
                        }
                    }
                },

                //password reset email
                new EmailTemplateLocalization
                {
                    Uuid = Guid.Parse("0e172a1a-a4f3-4058-9456-ef5af61b778f"),
                    Name = "Password reset request",
                    Description =
@"Email sent when user cannot login and decides to reset his pass.
Replacement tokens are:
    {VerificationKey},
    {UserName},
    {Email}
    {RedirectUrl}
",
                    Identifier = "pass_reset_request",
                    IsBodyHtml = true,
                    Translations = new EmailTranslations
                    {
                        { "en",
                            new EmailTemplate{
                                Title = "[no-reply@maphive.net] MapHive account password reset requested for {UserName}.",
                                Body =
$@"<h3>Hi {{UserName}}</h3>
<p>MapHive account password reset has been requested for your account. If you have not requested a password reset then please ignore this email as nothing has changed for you.<br/>Otherwise please follow the instructions below to complete a password reset request.</p>
<br/>
<p>In order to reset your current password please click <a href=""{resetPassLink}"">here</a> or paste the following link in your browser: {resetPassLink}</p>
<br/>
<p>Kind regards<br/>MapHive Team</p>"
                            }
                        }
                    }
                }
            };
        }
    }
}
