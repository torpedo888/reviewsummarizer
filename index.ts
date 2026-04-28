import concurrently from 'concurrently';

concurrently(
  [
    {
      name: 'server',
      command: 'dotnet watch run --urls http://localhost:3000',
      cwd: 'packages/server-dotnet',
      prefixColor: 'cyan',
    },
    {
      name: 'client',
      command: 'bun dev',
      cwd: 'packages/client',
      prefixColor: 'magenta',
    },
  ],
  {
    prefix: 'name',
    killOthers: ['failure', 'success'],
  }
).result?.then(success, failure);

function success() {
  console.log('All processes completed successfully.');
}

function failure() {
  console.error('One or more processes failed.');
}
