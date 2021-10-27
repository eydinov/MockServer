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
	"Name": "Test mock movie endpoint",
	"Request": {
		"Method": "GET",
		"Path": "/api/v1/movie/moneyheist"
	},
	"Response": {
		"Status": 200,
		"Headers": {
			"Content-Type": "application/json"
		},
		"Body": {
			"Type": "inline",
			"Props": {
				"Body": "{\"name\":\"La casa de papel\",\"year\":2017,\"seasons\":5,\"genre\":[\"Crime\",\"Drama\",\"Heist\",\"Thriller\"]}"
			}
		},
		"Delay": 0
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
With this option MockServer simply compares whether the Method and PATH configured in mock.json file match the URL client requested. If they are equal the response will be returned.

```json
{
	"Name": "Test mock movie endpoint",
	"Request": {
		"Method": "GET",
		"Path": "/api/result"
	},
	"Response": {
		"Status": 200,
		"Headers": {
			"Content-Type": "application/json"
		},
		"Body": {
			"Type": "inline",
			"Props": {
				"Body": "{\"result\":\"true\"}"
			}
		},
		"Delay": 0
	}
}
```

![path](https://user-images.githubusercontent.com/93197903/139099728-8f6bfc3c-5fc0-4a26-90dc-c9c11a289984.png)

### Matching by Method and PathRegex
With this option you can use regular expressions to match the PATH. If MockServer will successfully match regex expression with the URL client requested response will be returned.

```json
{
	"Name": "Test mock endpoint with regex",
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
			"Type": "inline",
			"Props": {
				"Body": "{\"result\":true}"
			}
		},
		"Delay": 0
	}
}
```
![pathregex](https://user-images.githubusercontent.com/93197903/139103594-7972339a-7303-44e0-877a-0bb7af73f17c.png)
