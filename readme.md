# Setup

1. Install [Docker](https://www.docker.com)

2. Clone the repo and Build MS SQL Server container using next commands:
```
git clone https://github.com/aChainsmoker/MoneyManager.git
cd MoneyManager/
docker compose up --build -d
```

3. Start the App
```
dotnet run --project MoneyManager.Main/
```
