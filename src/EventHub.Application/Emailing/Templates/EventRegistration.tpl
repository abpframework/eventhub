<html>

<head>
    <meta name="viewport" content="width=device-width">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
    <title>Eventhub</title>
    <style>
        @media only screen and (max-width: 620px) {
                table[class=mail-body] h1{
	                font-size: 36px !important;
	                margin-bottom: 10px !important;
                }
                table[class=mail-body] .content {
	                padding: 0 !important;
                }
                table[class=mail-body] .container {
	                padding: 0 !important;
	                width: 100% !important;
                }
                table[class=mail-body] .main {
	                border-left-width: 0 !important;
	                border-radius: 0 !important;
	                border-right-width: 0 !important;
                }
                table[class=mail-body] td {
	                width: 100% !important;
	                float: left!important;
                }
                table[class=mail-body] td a.btn {
	                display: block !important;
	                text-align: center;
                }
                table[class=mail-body] td h2 {
	                margin: 10px 0 !important;
                }
                table[class=mail-body] td .cont {
	                padding: 30px !important;
                }
            }
    </style>
</head>

<body>
    <table border="0" cellpadding="0" cellspacing="0" class="mail-body" style="background-color: #ffffff;width: 100%; font-size: 16px;line-height: 1.4;font-family: Arial, sans-serif;">
        <tbody>
            <tr>
                <td class="container" width="800" style="display: block;margin: 0 auto !important; max-width: 800px;padding: 10px;width: 800px;">
                    <div class="content" style="box-sizing: border-box;display: block;margin: 0 auto;max-width: 800px;padding: 0px;">
                        <div style="display: flex; padding-top: 1.5rem">

                            <h1 style="justify-content: center;margin-left: auto;margin-right: auto;align-content: center;">EventHub</h1>

                        </div>
                        <!-- START CENTERED WHITE CONTAINER -->
                        <div style="display: flex;">

                            <h1 style="justify-content: center;margin-left: auto;margin-right: auto;align-content: center;">{{model.title}}</h1>

                        </div>
                        <table class="main" style="background: #ffffff;border-radius: 3px; width: 100%;">
                            <!-- START MAIN CONTENT AREA -->
                            <tbody>
                                <tr>
                                    <td class="wrapper" width="800">
                                        <table class="main" style="background: #ffffff;border-radius: 3px;width: 100%;">
                                            <tbody>
                                                <tr>
                                                    <td class="wrapper" width="800">
                                                        <table border="0" cellpadding="0" cellspacing="0" style="border-collapse: separate; vertical-align: top;mso-table-lspace: 0pt;mso-table-rspace: 0pt;width: 100%; ">
                                                            <tbody>
                                                                <tr>
                                                                    <td style="vertical-align: top;" width="100%">
                                                                        <div class="cont" style="background: #eeeeee; border-radius: 10px; padding: 40px; margin-top: 20px;">
                                                                            <table style="border-collapse: separate; vertical-align: top;mso-table-lspace: 0pt;mso-table-rspace: 0pt;width: 100%; ">
                                                                                <tbody>
                                                                                    <tr>
                                                                                        <td width="100%" style="vertical-align: top; font-size: 16px; font-family: Arial, sans-serif; color: #444; margin-top: 20px; margin-bottom: 20px; line-height: 1.75">
                                                                                            <p>
                                                                                                Hi,
                                                                                            </p>
                                                                                            <p>
                                                                                                Great news! You attended the "{{model.title}}" event.
                                                                                            </p>
                                                                                            <div>
                                                                                                <table border="0" cellpadding="0" cellspacing="0" style=" border-collapse: separate; vertical-align: top; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%;">
                                                                                                    <tbody>
                                                                                                        <tr style="text-align: center">
                                                                                                            <td style="vertical-align: top;">

                                                                                                                <p style="font-size: 13px; font-family: Arial, sans-serif; color: #777777; margin: 0; margin-top: 30px;">Start Time</p>
                                                                                                                <p style="font-size: 21px; font-family: Arial, sans-serif; color: #444444; margin-bottom: 30px; margin-top: 6px;">{{model.start_time}}</p>
                                                                                                            </td>
                                                                                                            <td style="vertical-align: top;">

                                                                                                                <p style="font-size: 13px; font-family: Arial, sans-serif; color: #777777; margin: 0; margin-top: 30px;">End Time</p>
                                                                                                                <p style="font-size: 21px; font-family: Arial, sans-serif; color: #444444; margin-bottom: 30px; margin-top: 6px;">{{model.end_time}}</p>
                                                                                                            </td>

                                                                                                        </tr>
                                                                                                    </tbody>
                                                                                                </table>
                                                                                                <p style="margin-bottom: 20px; margin-top: 20px; text-align: center;">
                                                                                                    <a href="/events/{{model.url}}" class="btn" target="_blank" style="padding: 15px; background: #e90052; border-radius: 40px; text-align: center; color: white; font-weight: bold; text-decoration: none; font-size: 18px; padding-left: 45px; padding-right: 45px;">Event Details</a>
                                                                                                </p>
                                                                                            </div>

                                                                                            <p style="font-size: 16px; font-family: Arial, sans-serif; color: #444; margin-top: 20px; margin-bottom: 0px;">
                                                                                                Thank you,<br> EventHub
                                                                                            </p>
                                                                                        </td>
                                                                                    </tr>
                                                                                </tbody>
                                                                            </table>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>

                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <!-- START FOOTER -->
                        <table border="0" cellpadding="0" cellspacing="0" style="border-collapse: separate; vertical-align: top;mso-table-lspace: 0pt;mso-table-rspace: 0pt;width: 100%;">
                            <tbody>
                                <tr>
                                    <td class="content-block" style="vertical-align: top;" width="800">
                                        <div class="cont" style="background: #ffffff;border-top: 2px solid #eee;  padding: 35px 40px 5px; margin-top: 0px; text-align: center;">
                                            <a href="https://eventhub.abp.io/" style="font-size: 16px;  font-family: Arial, sans-serif;  color: #777;  font-weight: bold; margin-right: 30px; text-decoration: none;">Home</a>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="content-block powered-by" style="vertical-align: top;" width="100%">
                                        <div class="cont" style="background: #ffffff; border-radius: 10px; padding: 5px 40px; margin-top: 10px; margin-bottom: 50px; text-align: center;">
                                            <a href="mailto:info@eventhub.abp.io" style="font-size: 16px;  font-family: Arial, sans-serif;  color: #777;  font-weight: bold; margin-right: 30px; text-decoration: none;"><img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABgAAAARCAYAAADHeGwwAAABMElEQVQ4je3TvyvFYRQG8M+9LEwWiV0xyeAi2ZTUFQuZDAqTxUCJzY9BFotS/gAySAZFEgYlySzFLosfs169w6XL/V6uzbO973vO85z3OeekstlsGRYxrrTYw1g56jGNO6yivAQyS5hFdRpHGMQauvDwC+IXDGAGnTgJApXYxAJO0YLrH5Dfog3bmMAhatI5AUF1B4/oiIFJcYAMbrCRa3X6E0EvzlEbbZtLILCMHlQESzCS+/hZIKARF7Ef8+jDc564VwxhCq24jL/4gHwCAVXYxyR2o7e3Oe/30cbQuzEcB7/zEX03kkF8BU2RJFS3hVS07wnrGP2GI9HMD6MB/eiOd9Wx6vZCyUmXKhM9Hojn8JO6JInFbG2YrLMi4t/xVZNLhn+BgghNvkLzn7Bz9QbIADMmEt9imAAAAABJRU5ErkJggg==" width="16"> info@eventhub.abp.io</a>
                                            <span style="font-size: 16px;  font-family: Arial, sans-serif;  color: #777;  margin-right:  0px; text-decoration: none;"><strong>EventHub</strong> - Powered by Volosoft</span>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <!-- END FOOTER -->
                    <!-- END CENTERED WHITE CONTAINER -->
                </td>
            </tr>
        </tbody>
    </table>
</body>

</html>