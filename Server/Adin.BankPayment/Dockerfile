FROM microsoft/aspnetcore:2.0-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0-nanoserver-1709 AS build
WORKDIR /src
COPY Adin.BankPayment.sln ./
COPY Adin.BankPayment/Adin.BankPayment.csproj Adin.BankPayment/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/Adin.BankPayment
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Adin.BankPayment.dll"]
