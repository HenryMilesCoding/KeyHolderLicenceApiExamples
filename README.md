
# KeyHolderLicenceApiExamples
Here you will find concrete application examples for the KeyHolder and the HenryMilesLicenceAPI.

> In the near future, programs will be available here that clearly
> explain the use of the API and the KeyHolder. It's easier than you
> think.




## Create and check a license
```mermaid
sequenceDiagram
KeyHolder ->>+Our API: Command: Create account
Our API-->>-KeyHolder: Answer: Accountdata in JSON

KeyHolder ->>+Our API: Command: Create license
Our API-->>-KeyHolder: Answer: Storage locally retrievable

KeyHolder -->> Your product: Give license to customer to enter

Your product->>+Our API:  Request: Checking a license with system 
Our API-->>-Your product: Answer: Success or failure model
```