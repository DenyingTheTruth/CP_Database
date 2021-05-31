# report-app-backend
## For publish on linux Debian
```sh
1. $ git clone https://github.com/Caspel-LLC/report-app-backend.git
   $ git pull
```
```sh
2. dotnet publish -c Release -r linux-x64 -o /var/www/report-backend/ -f netcoreapp5.0
```
```sh
3. sudo systemctl restart kestrel-report-backend.service
```
