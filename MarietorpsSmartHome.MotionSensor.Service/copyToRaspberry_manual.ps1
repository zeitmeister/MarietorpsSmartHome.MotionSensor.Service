#Install-Module -Name Posh-SSH
#(Get-Command New-PSSession).ParameterSets.Name
Set-Location C:\Users\simon\hobby_projects\MarietorpsSmartHome.MotionSensor.Service\published
scp -r ./* pi@192.168.0.107:/home/pi/bajs
#$username = 'pi'
#$passwd = '0readYmade1!' | ConvertTo-SecureString -Force -AsPlainText
#$cred = New-Object -typename System.Management.Automation.PSCredential $username, $passwd
#Write-Host $cred.UserName
#$s = New-PSSession -HostName 192.168.0.107 -Credential $cred
#$s = New-PSSession -HostName '192.168.0.107' -Credential $cred -Port 22

#Copy-Item -Path "C:\Users\simon\hobby_projects\MarietorpsSmartHome.MotionSensor.Service\published" -Destination /home/pi/bajsddd -Force -Recurse -ToSession $s
Read-Host -Prompt "Press Enter to exit"