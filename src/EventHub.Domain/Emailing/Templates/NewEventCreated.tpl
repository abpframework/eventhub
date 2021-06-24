<h1
	style="font-size: 40px;  font-family: Arial, sans-serif;  color: #00044a;  font-weight: bold;  line-height: 1.2; margin-top: 50px;">
	{{model.title}}</h1>
<p style="font-size: 24px; font-family: Arial, sans-serif;  color: #00044a;">
	<strong>Hi {{model.user_name}},</strong></p>
<p style="font-size: 18px; font-family: Arial, sans-serif;  color: #666; margin-top: 20px; margin-bottom: 20px;">
	A new event called "{{model.title}}" has been created.
</p>
<p style=" margin-top: 30px; margin-bottom: 30px;">
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
			<a href="{{app_url}}/events/{{model.url}}" class="btn" target="_blank" style="padding: 15px; background: #e90052; border-radius: 40px; text-align: center; color: white; font-weight: bold; text-decoration: none; font-size: 18px; padding-left: 45px; padding-right: 45px;">Event Details</a>
		</p>
	</div>
</p>