# NET 5 - RabbitMQ Publish Excel

There is a vehicle list table in the user's database. According to the user's request, the data in this table are converted to an excel file. In the web project, the request to convert to Excel is directed to the worker service. The worker service notifies the request to the web project again after it has been processed. After the notification on the web side, the user is notified with sweet alert with signalR.

![Screenshot_1](https://user-images.githubusercontent.com/54249736/118404776-fbb3da00-b67c-11eb-8343-55737a072e7d.png)

## Libraries
![asdasdasd](https://user-images.githubusercontent.com/54249736/118404839-54837280-b67d-11eb-9764-78fdf7bdfb3b.png)

#### Installed Packages (PublishExcel.Shared)
* No packages found.

#### Installed Packages (PublishExcel.Web)
* Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
* Microsoft.EntityFrameworkCore.SqlServer
* Microsoft.EntityFrameworkCore.Tools
* Microsoft.VisualStudio.Web.CodeGeneration.Design
* RabbitMQ.Client
* Microsoft.AspNetCore.Identity.EntityFrameworkCore

#### Installed Packages (PublishExcel.WorkerService)
* ClosedXML
* Microsoft.EntityFrameworkCore
* Microsoft.EntityFrameworkCore.SqlServer
* Microsoft.EntityFrameworkCore.Tools
* Microsoft.Extensions.Hosting
* RabbitMQ.Client

#### Contact Addresses
##### Linkedin: [Send a message on Linkedin](https://www.linkedin.com/in/cihatsolak/) 
##### Twitter: [Go to @cihattsolak](https://twitter.com/cihattsolak)
##### Medium: [Read from medium](https://cihatsolak.medium.com/)
