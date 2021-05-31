# Report App

## Rules contributing

!!! NOT PUSH COMMITS IN `master` OR `develop` DIRECTLY !!!

1. Create new branch from `develop` 
2. Work in your branch (commit & implement tasks)
3. After implementing some tasks create Pull Request from your branch to `develop`

## API's troubleshooting

If you can't log in report app then try open [Swagger API](https://10.81.80.100:9999/swagger/index.html) and confirm following a link

![image](https://user-images.githubusercontent.com/43136703/109479927-fab2d980-7a8b-11eb-8d0b-777bf2289fe5.png)

After that API request not blocking by CORS-policy and the application should work.

## Release report-app to dev server

1. In file `constants.ts` use development URL

![image](https://user-images.githubusercontent.com/43136703/109478870-d3a7d800-7a8a-11eb-95b3-f79944832205.png)

2. Build react project

`yarn win-build`

3. Copy build folder to remote dev server

`scp -r {full path to build folder on your PC} user@10.81.80.100:/var/www/report-app/`

SSH's password - `Qwerty1`

Example:

`scp -r C:/Users/User/Desktop/Work/report-app-frontend/build user@10.81.80.100:/var/www/report-app/`

## Release report-app to production server

1. In file `constants.ts` use production URL

![image](https://user-images.githubusercontent.com/43136703/109478913-defb0380-7a8a-11eb-9707-76705bb379b8.png)

2. Build react project

`yarn win-build`

3. Copy build folder to remote dev server

`scp -r {full path to build folder on your PC} user@195.50.4.193:/var/www/report-app/`

SSH's password - `n6GBpmR2uFvw`

Example:

`scp -r C:/Users/User/Desktop/Work/report-app-frontend/build user@195.50.4.193:/var/www/report-app/`
