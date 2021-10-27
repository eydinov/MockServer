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
