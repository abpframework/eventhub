       <tr width="680">
            <td width="680" style="width: 680px;">
                <table style="padding-bottom: 50px; padding-top: 50px; width: 680px; font-family:Arial, Helvetica, sans-serif; font-size:16px; color: #00044A; border-top: 2px solid rgba(41, 45, 51, 0.08); border-bottom: 2px solid rgba(41, 45, 51, 0.08);">
                    <tr>
                        <td>
                            <h1 style="margin-top: 0; font-size: 30px;">
                                Hey {{model.user_name}},
                                <br />
                                Just a friendly reminder to you that the event is soon! Looking forward to seeing you in the event!
                            </h1>
                            <img src="{{model.image_url}}" />
                            <br /><br /><br />
                            <h1 style="margin-top: 0; font-size: 30px;"> {{model.title}} </h1>
                            <span style="color: #6F32E2;">{{model.start_time}} - {{model.end_time}}   |   {{model.address}}</span>
                            <p style="font-size: 22px; line-height: 37px; color: #6B7E92;"> {{model.description}} </p>
                        </td>
                    </tr>
                    <tr>
                        <td>
                          <a href="{{app_url}}/events/{{model.url}}" style="display: inline-block; border-radius: 10px; padding:12px 20px; background-color: #E8345D; text-decoration: none; color: #fff;" target="_blank">Details</a>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>