# MockServer
Mock server written on .net core
## What the MockServer is?
MockServer allows you to mock any server or service via HTTP or HTTPS, such as a REST or RPC service.

It is useful in the following scenarios:

**Testing**

- Easily recreate all types of responses for HTTP services such as REST or RPC services to test applications easily and affectively
- Isolate the system-under-test to ensure tests run reliably and only fail when there is a genuine bug. It is important only the system-under-test is tested and not its dependencies to avoid tests failing due to irrelevant external changes such as network failure or a server being rebooted / redeployed.
- Easily setup mock responses independently for each test to ensure test data is encapsulated with each test. Avoid sharing data between tests that is difficult to manage and maintain and risks tests infecting each other
- Create test assertions that verify the requests the system-under-test has sent

**De-coupling development**

- Start working against a service API before the service is available. If an API or service is not yet fully developed MockServer can mock the API allowing any team who is using the service to start work without being delayed
- Isolate development teams during the initial development phases when the APIs / services may be extremely unstable and volatile. Using MockServer allows development work to continue even when an external service fails

## How it works?
When MockServer receives a request it matches the request against active expectations that have been configured, if no matches are found a 404 (NotFound) is returned.

MockServer written in .Net core therefore it is crossplatform by the nature and can be run either on windows or linux machine. It’s is super light weight and fast :)

## Configuration
To keep the mocking simple here is an example configuration used for setting up a mock HTTP endpoint. It has a “Request” where you configure the matching conventions and a “Response” where you put in the results you want from the server.

All definitions for mocked services are stored into Configuration folder within mock.json file
### Example Mocks setup via mock.json file

```json
{
	"name": "Test mock movie endpoint",
	"request": {
		"method": "GET",
		"path": "/api/v1/movie/moneyheist"
	},
	"response": {
		"status": 200,
		"headers": {
			"content-Type": "application/json"
		},
		"body": {
			"type": "inline",
			"props": {
				"body": "{\"name\":\"La casa de papel\",\"year\":2017,\"seasons\":5,\"genre\":[\"Crime\",\"Drama\",\"Heist\",\"Thriller\"]}"
			}
		},
		"delay": 0
	}
}
```

## Start MockServer
MockServer can be run:

- Manually. You need to build the solutuion and run MockServer.exe under build directory.
- By using Docker container (TBD - not yet available)

## MockServer UI:
MockServer has an inbuilt UI that can be used to view the list of mocked services implemented within MockServer.

![mock_ui](https://user-images.githubusercontent.com/93197903/139095712-3b74e5d6-fdcb-4b01-a92a-55acb0573205.png)

## Matching options

### Matching by Method and Path
With this option MockServer simply compares whether the METHOD and PATH configured in mock.json file match the METHOD and URL client app requested. If they are equal the response will be returned.

```json
{
	"name": "Mock movie endpoint",
	"request": {
		"method": "GET",
		"path": "/api/result"
	},
	"response": {
		"status": 200,
		"headers": {
			"content-Type": "application/json"
		},
		"body": {
			"type": "inline",
			"props": {
				"body": "{\"result\":\"true\"}"
			}
		},
		"delay": 0
	}
}
```

![path](https://user-images.githubusercontent.com/93197903/139099728-8f6bfc3c-5fc0-4a26-90dc-c9c11a289984.png)

### Matching by Method and PathRegex
With this option regular expressions can be used to match the PATH. If MockServer successfully matches regex expression with the URL client requested response will be returned.

```json
{
	"name": "Mock endpoint with PathRegex",
	"request": {
		"method": "GET",
		"pathRegex": "/payment/v1/stores/(\\d+)/order/(\\d+)$"
	},
	"response": {
		"status": 200,
		"headers": {
			"content-Type": "application/json"
		},
		"body": {
			"type": "inline",
			"props": {
				"body": "{\"result\":true}"
			}
		},
		"delay": 0
	}
}
```
![pathregex](https://user-images.githubusercontent.com/93197903/139103594-7972339a-7303-44e0-877a-0bb7af73f17c.png)

With PathRegex it is also possible to use all found matches and substitute the response content with them. In order to use functionality be sure that response file contains properly defined replacement string. Please have a look [official regex documentation](https://docs.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex.replace?view=net-5.0)

```json
{
	"name": "Mock endpoint with PathRegex",
	"request": {
		"method": "GET",
		"pathRegex": "/payment/v1/stores/(\\d+)/order/(\\d+)$"
	},
	"response": {
		"status": 200,
		"headers": {
			"content-Type": "application/json"
		},
		"body": {
			"type": "inline",
			"props": {
				"body": "{\"result\":{\"store\":\"$1\",\"order\":\"$2\"}}"
			}
		},
		"delay": 0
	}
}
```

![pathregexbody](https://user-images.githubusercontent.com/93197903/139256036-d60df6df-b684-4bc0-b173-e330e81c7c51.png)

## Response body options
At the moment, three types of responses have been implemented:
- Inline response type
- File response type
- Assembly response type

### Inline response type
Inline response type allows to define the response directly in mock.json file.
With body type = "inline" the props "body" must be defined.

```json
"response": {
        "status": 200,
        "headers": {
          "content-Type": "application/json"
        },
        "body": {
          "type": "inline",
          "props": {
            "body": "{\"actor\":{\"firstName\":\"Ursula\",\"lastName\":\"Corbera\"}}"
          }
        },
        "delay": 0
      }
```

### File response type
File response type allows to define the response inside a separate file on file system.
With body type = "file" the props "fileName" must be defined.

```json
"response": {
        "status": 200,
        "headers": {
          "content-Type": "application/json"
        },
        "body": {
          "type": "file",
          "props": {
            "fileName": "\\responses\\orders\\035_2161190600113165.json"
          }
        },
        "delay": 0
      }
```

With using PathRegex you can also define a filename with a regular expression replacement string inside it as shown in the example below. This option allows to define different set of responses without having to modify the mock.json file

```json
{
	"name": "Mock endpoint with file response type and PathRegex",
	"request": {
		"method": "GET",
		"pathRegex": "/payment/v1/stores/(\\d+)/order/(\\d+)$"
	},
	"response": {
		"status": 200,
		"headers": {
			"content-Type": "application/json"
		},
		"body": {
			"type": "file",
			"props": {
				"fileName": "\\responses\\orders\\$1_$2.json"
			}
		},
		"delay": 0
	}
}
```

### Assembly response type
Assembly response type allows to implement the way and form of the response inside a separate assembly file (.net library) called mock plugin. Mock plugin it is a class implementing IMockPlugin interface.

With body type = "assembly" the props "assembly" must be defined. Props "class" should be defined if there are more than one IMockPlugin implementations in one project. If class props is not defined the first one will be taken. Other props can be configured if plugn implementation requires them. You can pass as many props as your implementation requires.

```json
{
	"name": "Mock Server dashboard",
	"request": {
		"method": "GET",
		"path": "/"
	},
	"response": {
		"status": 200,
		"headers": {
			"content-Type": "text/html; charset=UTF-8"
		},
		"body": {
			"type": "assembly",
			"props": {
				"assembly": "\\plugins\\dashboard\\dashboard.dll",
				"class": "Dashboard",
				"page": "\\responses\\dashboard\\index.html"
			}
		},
		"delay": 0
	}
}
```

### Delay
Mock servers often return responses faster than their real API counterparts. This is great when you just want your functional test suite to run as fast as possible, but if you’re testing your app’s UX with realistic timings, or want to check that a timeout is configured correctly then you need to be able to add artificial delay to your responses.

MockServer allows you to setting up response in a way that it can have a fixed delay attached to it, such that the response will not be returned until after the specified number of milliseconds:

```json
{
      "Name": "Test mock endpoint with 5000ms delay",
      "Request": {
        "Method": "GET",
        "PathRegex": "/payment/v1/stores/(\\d+)/order/(\\d+)$"
      },
      "Response": {
        "Status": 200,
        "Headers": {
          "Content-Type": "application/json"
        },
        "Body": {
          "type": "file",
          "props": {
            "fileName": "\\responses\\orders\\035_2161190600113165.json"
          }
        },
        "Delay": 5000
      }
    }
```
![delay](https://user-images.githubusercontent.com/93197903/139413978-35effba0-b251-494a-ba47-0d8c6c6db0d2.png)

## Authorization and authentication
In real life almost all API your application would be communicating with will be secured by authorization and authentication mechanisms.
At the moment three types of authentication are supported:
- HTTP Basic authentication
- Bearer token authentication
- Header authentication


### HTTP Basic authentication
HTTP Basic is a widely supported part of the HTTP standard supporting username/password authentication. An HTTP resource secured with HTTP Basic will result in a browser prompting the user with a username/password dialogue box on their initial visit.

In order to setup Mock API to use Basic authentication schema authorization object should be properly configured inside the mock.json file for each particular API.

```json
{
      "name": "Endpoint secured with HTTP Basic authentication",
      "request": {
        "method": "GET",
        "path": "/HttpService/Contractor",
        "authorization": [
          {
            "schema": "Basic",
            "unauthorizedStatus": 401,
            "unauthorizedMessage": "Not authorized",
            "claims": {
              "userName": "testadmin",
              "password": "Qwerty123",
              "realm": "test:realm:server"
            }
          }
        ]
      },
      "response": {
        "status": 200,
        "headers": {
          "content-Type": "text/text; charset=UTF-8"
        },
        "body": {
          "type": "inline",
          "props": {
            "body": "customerID = 6231239765420; customerName = Ursula Corbero; customerBIN = 9999999999; customerCountry = Spain; customerGovernment = False"
          }
        },
        "delay": 0
      }
}
```
Once you reach out the page the BASIC authentication mechanism will request you to enter login and password

![basic](https://user-images.githubusercontent.com/93197903/139422685-1b2c6925-d5fa-4a6c-8e40-01ec159dbd22.png)

If enter wrong credential API will return the HTTP Status code 401 and the message "Not authorized" as configured in the API configuration file.

![basic_fail](https://user-images.githubusercontent.com/93197903/139422834-838f4231-a13c-46c6-8bbd-8b16315a108b.png)

If enter correct credentials you call will be successfully authorized therefore you will be received with the correct response and HTTP status code 200

![basic_success](https://user-images.githubusercontent.com/93197903/139422989-7572f4cc-27fe-40b1-82c8-d2f69ecebe53.png)

### Bearer authentication
Bearer authentication (also called token authentication) is an HTTP authentication scheme that involves security tokens called bearer tokens. The name “Bearer authentication” can be understood as “give access to the bearer of this token.” The bearer token is a cryptic string, usually generated by the server in response to a login request.

MockServer emulates OAuth2 authentication logics and checks against the validity of the token. Additinaly you can set in configuration which Claims must be checked inside the Bearer token to make a decision that token is valid or not.

```json
{
      "mame": "Weather endpoint secured by Bearer",
      "request": {
        "method": "GET",
        "pathRegex": "/api/weather/city/([a-zA-Z]+)$",
        "authorization": [
          {
            "schema": "Bearer",
            "unauthorizedStatus": 401,
            "unauthorizedMessage": "HTTP Error 401 - Unauthorized: Access is denied",
            "claims": {
              "appid": "030f0c52-792a-4086-ab25-9565dcebd350",
              "unique_name": "testadmin"
            }
          }
        ]
      },
      "response": {
        "status": 200,
        "headers": {
          "content-Type": "application/json"
        },
        "body": {
          "type": "inline",
          "props": {
            "body": "{\"forecasts\":[{\"day\":\"Sun\",\"date\":1547366400,\"low\":48,\"high\":58,\"text\":\"Rain\",\"code\":12},{\"day\":\"Mon\",\"date\":1547452800,\"low\":47,\"high\":58,\"text\":\"Rain\",\"code\":12},{\"day\":\"Tue\",\"date\":1547539200,\"low\":46,\"high\":59,\"text\":\"Scattered Showers\",\"code\":39},{\"day\":\"Wed\",\"date\":1547625600,\"low\":49,\"high\":56,\"text\":\"Rain\",\"code\":12},{\"day\":\"Thu\",\"date\":1547712000,\"low\":49,\"high\":59,\"text\":\"Scattered Showers\",\"code\":39},{\"day\":\"Fri\",\"date\":1547798400,\"low\":48,\"high\":61,\"text\":\"Showers\",\"code\":11},{\"day\":\"Sat\",\"date\":1547884800,\"low\":47,\"high\":62,\"text\":\"Rain\",\"code\":12}]}"
          },
          "delay": 0
        }
      }
    }
```

If try to call such API without bearer token provided the HTTP Status code 401 will be returned together with response message configured in mock.json configuration file.

![bearer_no_token](https://user-images.githubusercontent.com/93197903/139437075-bb59a7e7-c7a1-403f-af41-fa62a35c2607.png)

If provide not valid token to the response e.g. token wich has been expired or haven't passed the validation process against the Claims which were setup for the API the HTTP Status code 403 (Forbidden) will be returned.

![bearer_no_token](https://user-images.githubusercontent.com/93197903/139437819-8b686e3b-703a-4de8-a2aa-e60274fc0be6.png)

And eventually if token is correct and consists all expected Claims API will return the response

![bearer_success](https://user-images.githubusercontent.com/93197903/139437979-99fcc95c-cad2-4b5c-b3df-381ad5e92997.png)

### OAuth token provider
To communicate with API secured by Bearer authentication schema application should first request an access to the API by requesting the token from token provider. MockServer does not stick to any specific provider - you are free to use any provider you want or have access to.

But sometimes communication with token provider is a part of integration process and it could be happened that token provided is not accessible from the environment where you develop or test API. For such scenarios MockServer provides its own already mocked OAuth2 token provider emulator.

It has been already configured inside the mock.json file:

```json
 {
      "Name": "Mock OAuth2 provider",
      "Request": {
        "Method": "POST",
        "Path": "/oauth2/token",
        "Props": {
          "grant_type": "password",
          "username": "testadmin",
          "password": "Qwerty123",
          "client_id": "030f0c52-792a-4086-ab25-9565dcebd350",
          "client_secret": "0_T-Gq-HY8vcfFTjIZkWrddimfUIACgm7PEkEPoI"
        }
      },
      "Response": {
        "Status": 200,
        "Headers": {
          "Content-Type": "application/json"
        },
        "Body": {
          "Type": "assembly",
          "Props": {
            "Assembly": "\\plugins\\oauth\\OAuth.dll",
            "Class": "OAuth",
            "Body": "{\"access_token\": \"{access_token}\", \"token_type\": \"bearer\", \"expires_in\": {expires_in}, \"refresh_token\": \"{refresh_token}\", \"refresh_token_expires_in\": {refresh_expires_in}}",
            "Claims": "{\"appid\":\"{appid}\", \"apptype\":\"{apptype}\", \"authmethod\":\"{authmethod}\", \"unique_name\":\"{unique_name}\", \"upn\":\"{upn}\"}"
          }
        },
        "Delay": 0,
        "Props": {
          "aud": "https://oauth2server",
          "iss": "https://oauth2server",
          "exp": "3600",
          "appid": "030f0c52-792a-4086-ab25-9565dcebd350",
          "apptype": "Confidential",
          "authmethod": "urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport",
          "unique_name": "testadmin",
          "upn": "testadmin@mycompany.com"
        }
      }
    }
```
