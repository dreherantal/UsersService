# Users Service

Simple user service for a prototype microservice

still in development

## Description

API key and UserId validation from headers

MSSQL server as database

custom access token generation and refresh token generation/validation

refresh tokens are stored per user for multiple user logins

Serilog with correaltionID in file sink

register, login, refresh token and validation(for testing purpose) enpdoints

### Dependencies

docker compose for MSSQL attached

EF database needs to be created

best to use is with the API Gateway part of microservice (soon will be uploaded)

### To do list

Add SEQ instead of file sink for Serilog

Add fluentvalidation for requests

Implemenet user roles and permissions


## Acknowledgments
* [best password hashing/verifying helper]https://stackoverflow.com/questions/4181198/how-to-hash-a-password/73125177#73125177
