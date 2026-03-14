# syntax=docker/dockerfile:1.7
ARG SDK_IMAGE=mcr.microsoft.com/dotnet/sdk:10.0
ARG RUNTIME_IMAGE=mcr.microsoft.com/dotnet/aspnet:10.0

FROM ${SDK_IMAGE} AS build
ARG PROJECT_PATH
WORKDIR /src
COPY . .
RUN dotnet publish "${PROJECT_PATH}" -c Release -o /app/publish

FROM ${RUNTIME_IMAGE} AS runtime
ARG ENTRY_DLL
ENV ENTRY_DLL=${ENTRY_DLL}
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["sh", "-c", "dotnet \"$ENTRY_DLL\""]

