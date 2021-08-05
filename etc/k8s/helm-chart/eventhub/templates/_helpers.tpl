{{- define "eventhub.global.env" -}}
- name: "AppUrls__Account"
  value: "{{ .Values.global.accountUrlFull }}"
- name: "AppUrls__Www"
  value: "{{ .Values.global.wwwUrlFull }}"
- name: "AppUrls__Api"
  value: "{{ .Values.global.apiUrlFull }}"
- name: "AppUrls__Admin"
  value: "{{ .Values.global.adminUrlFull }}"
- name: "AppUrls__AdminApi"
  value: "{{ .Values.global.adminApiUrlFull }}"
{{- end }}