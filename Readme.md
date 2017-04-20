# Example Suave Application with Heroku Deploy

## Buildpacks used:

* https://github.com/fgNv/mono-script-buildpack.git

* heroku/nodejs (for frontend)

## In order to build the application:

### Paket and fsi must be registered in PATH, and npm must be installed

* paket restore

* fsi build.fsx

* npm install

## In order to run the application:

* fsi App.fsx

* (in another console) cd package-tracking-management-view && npm run dev