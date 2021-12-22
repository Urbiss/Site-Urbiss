FROM mcr.microsoft.com/dotnet/sdk:5.0
COPY . /usr/src/api
WORKDIR /usr/src/api/Urbiss.API
EXPOSE 5000
ENTRYPOINT [ "dotnet", "run" ]
