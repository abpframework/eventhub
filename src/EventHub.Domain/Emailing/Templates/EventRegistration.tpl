     <tr width="680">
            <td width="680" style="width: 680px;">
                <table
                    style="padding-bottom: 50px; padding-top: 50px; width: 680px; font-family:Arial, Helvetica, sans-serif; font-size:22px; color: #00044A; border-top: 2px solid rgba(41, 45, 51, 0.08); border-bottom: 2px solid rgba(41, 45, 51, 0.08);">
                    <tr>
                        <td>
                            <p style="line-height: 37px; color: #6B7E92;">Great news, {{model.full_name_or_user_name}}! You are attending to the "{{model.title}}" event.</p>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr style="vertical-align: top;">
                                    <td style="width:232px;" width="232">
                                        <img src="{{model.thumbnail_url}}" style="width: 232px; border-radius: 10px;">
                                    </td>
                                    <td style="padding-left: 15px;">
                                        <h4 style="margin-top:0; margin-bottom: 10px;">{{model.title}}</h4>
                                        <span style="color: #6F32E2; font-size: 16px;">{{model.start_time}} - {{model.end_time}} | {{model.location}}</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <p style="line-height: 37px; color: #6B7E92;">
                                {{model.description}}
                            </p>
                        </td>
                    </tr>

                    <tr>
                        <td style="text-align:center; padding-top:30px">
                            <a href="{{app_url}}/events/{{model.url}}" style="border-radius: 10px; padding:12px 30px; background-color: #E8345D; text-decoration: none; color: #fff;">
                                Details
                            </a>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
