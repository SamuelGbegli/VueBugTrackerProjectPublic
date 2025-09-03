# vuebugtrackerproject.client

This template should help get you started developing with Vue 3 in Vite.

## Recommended IDE Setup

[VSCode](https://code.visualstudio.com/) + [Volar](https://marketplace.visualstudio.com/items?itemName=Vue.volar) (and disable Vetur).

## Type Support for `.vue` Imports in TS

TypeScript cannot handle type information for `.vue` imports by default, so we replace the `tsc` CLI with `vue-tsc` for type checking. In editors, we need [Volar](https://marketplace.visualstudio.com/items?itemName=Vue.volar) to make the TypeScript language service aware of `.vue` types.

## Customize configuration

See [Vite Configuration Reference](https://vite.dev/config/).

## Project Setup

```sh
npm install
```

### Compile and Hot-Reload for Development

```sh
npm run dev
```

### Type-Check, Compile and Minify for Production

```sh
npm run build
```

### Lint with [ESLint](https://eslint.org/)

```sh
npm run lint
```

The application requires a functioning SQL database to work.

The following instructions are based on using Visual Studio 2022.

1. Open the SQL Server Object Explorer (click on View in the menu bar)
2. Expand a database server, right click on the Databases folder and click "Add New Database"
3. Give a name to the database, choose a location to save the database and click "OK"
4. Go to the properties section of the selected database, go to the connection string section and copy the string
5. Open the file appsettings.json in VueBugTracker.Server, and add the following to the "ConnectionStrings"section:
"Local": "{database connection string}"
6. Open the Package Manager Console (View -> Other Windows -> Package Manager Console) and type the following commands:
```sh
add-migration "{migration-name}"
update-database
```

To log in as a super user, enter the following in the login dialog:
Username: super user
Password: TestPassword1

Addding an SMTP server

The application uses a SMTP server to send emails to a recepiant. This is mainly used to simulate sending an email to reset a user's password.

The SMTP 