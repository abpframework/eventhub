<!doctype html>
<html>

<head>
    <meta name="viewport" content="width=device-width" />
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <title>EventHub</title>
</head>

<body style="margin:0; padding:0;" bgcolor="#fff" leftmargin="0" topmargin="0" marginwidth="0" marginheight="0">
    <table border="0" width="680" cellpadding="0" cellspacing="0" class="container" align="center"
        style="width:880px; max-width:680px; padding-top: 100px; padding-bottom: 100px;">
        <tr width="680">
            <td align="center" width="680" style="padding-bottom: 50px;">
                <a href="{{app_url}}">
                    <img src="data:image/png;base64,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" height="36">
                </a>
            </td>
        </tr>

		  {{ content }}
		  
        <tr>
            <td width="240" align="center" style="padding-top: 40px; width: 240px;">
                <table width="240" style="width: 240px;">
                    <tr>
                        <td width="240" align="center"
                            style="width: 240px; display: flex; justify-content: space-between;">
                            <a href="https://twitter.com/openeventhub" style="display: inline-block;">
                                <img src="{{app_url}}/assets/email/twitter.png"
                                    style="height: 52px;">
                            </a>
                            <a href="https://www.instagram.com/openeventhub/" style="display: inline-block;">
                                <img src="{{app_url}}/assets/email/instagram.png" style="height: 52px;">
                            </a>
                            <a href="https://www.facebook.com/openeventhub" style="display: inline-block;">
                                <img src="{{app_url}}/assets/email/facebook.png" style="height: 52px;">
                            </a>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>

        <tr>
              <td width="680" style="width: 680px;" align="center">
                <table width="680" align="center" style="width: 680px; font-family:Arial, Helvetica, sans-serif; font-size:14px;">
                    <tr>
                        <td width="680" align="center" style="width: 680px; padding-top:40px; padding-bottom:40px;">
                            <a href="https://www.openeventhub.com/" target="_blank" style="display: block; color:#E8345D; font-size: 16px; font-weight: 600; text-decoration: none; padding-bottom: 20px;">www.openeventhub.com</a>
                            <span style="font-size:14px; color: rgba(41, 45, 51, 0.6);">Copyright © 2021 - All rights reserved.</span>

                            <span style="display: block; padding-top: 25px;">
                                <a href="#" style="display: inline-block; background-color: rgba(41, 45, 51, 0.15); border-radius: 5px; text-decoration: none; color: rgba(41, 45, 51, 0.3); font-weight: 600; padding: 5px 10px; margin-right: 10px;">View in browser</a>
                                <a href="#" style="display: inline-block; background-color: rgba(41, 45, 51, 0.15); border-radius: 5px; text-decoration: none; color: rgba(41, 45, 51, 0.3); font-weight: 600; padding: 5px 10px ;">Unsubscribe</a>
                            </span>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>