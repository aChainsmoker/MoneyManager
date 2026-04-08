# Setup

1. Install [Docker](https://www.docker.com)

2. Build MS SQL Server container using next commands:
```
https://github.com/aChainsmoker/MoneyManager.git
cd MoneyManager/
docker compose up --build -d
```

3. Start App
```
dotnet run --project MoneyManager.Main/
```
