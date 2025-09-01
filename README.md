# Dapper vs. EF Core Performance Comparison - EN

![.NET 8](https://img.shields.io/badge/.NET-8-blue.svg)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

This project is a REST API designed to compare the performance of two of the most popular database access technologies in the .NET ecosystem: **Dapper** and **Entity Framework Core**.

The main goal of this project is to provide developers with concrete data that highlights the trade-off between *ease of use* and *raw performance*.

## 📊 Benchmark Summary

| Scenario          | EF Core Time | Dapper Time |
|-------------------|--------------|-------------|
| Data Insertion    | 6736 ms      | 395 ms      |
| Generate Orders   | 60607 ms     | -           |
| Report Top Sellers| 255 ms       | 224 ms      |

---

## 📸 Visual Results

### Data Insertion
**Entity Framework Core (6736 ms)**  
![EF Core Insert](https://github.com/user-attachments/assets/85b32c24-2423-4142-bb3e-975588be4b37)  

**Dapper (395 ms)**  
![Dapper Insert](https://github.com/user-attachments/assets/efdb4226-2e8a-4731-bf1d-7074769b8c79)  

---

### Generate Orders
**Entity Framework Core (60607 ms)**  
![EF Core Orders](https://github.com/user-attachments/assets/8fffdbe5-9474-4966-9933-4771993f06b5)  

---

### Report Top Sellers
**Entity Framework Core (255 ms)**  
![EF Core Report](https://github.com/user-attachments/assets/91952a5e-19f0-42d5-8a5b-ca18a7a49364)  

**Dapper (224 ms)**  
![Dapper Report](https://github.com/user-attachments/assets/12c6aaa7-9530-4254-9f40-5eaf268f6128)  


## 📈 Compared Scenarios

This benchmark project focuses on two critical scenarios where ORM performance differences are most noticeable:

1. **Bulk Data Insertion:**  
   A write-heavy scenario where tens of thousands of records are inserted into the database in a single operation.  
   This test measures the overhead of EF Core’s change tracking mechanism compared to Dapper’s raw performance with `SqlBulkCopy`.

2. **Complex Read Query (Reporting):**  
   A read-heavy scenario involving multiple table `JOIN`s, grouping (`GROUP BY`), and aggregation (`SUM`).  
   This test measures the cost of EF Core translating LINQ to SQL versus Dapper executing a hand-optimized SQL query directly.

&nbsp;

## 🛠️ Technology

* **Framework:** .NET 8  
* **API:** ASP.NET Core Web API  
* **Database:** SQL Server  
* **ORM / Micro-ORM:**
  * Entity Framework Core  
  * Dapper  
* **Data Generation:** Bogus (for realistic fake data)

&nbsp;

## Getting Started

Follow these steps to run the project locally and perform the benchmarks.

### Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
* [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)  
* [Git](https://git-scm.com/)

### Installation Steps

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/kayamuhammet/Dapper-vs-EFCore.git
   cd backend
    ```

2. **Configure Database Connection:**
   Open `appsettings.Development.json` and update the `ConnectionStrings` section with your SQL Server instance details.

   ```json
   {
       "ConnectionStrings": {
           "DefaultConnection": "Server=(LocalServerName)\\SQLEXPRESS;Database=DbName;Trusted_Connection=True;TrustServerCertificate=True;"
       }
   }
   ```

   > **Note:** Replace `Server` with your own SQL Server name. The database will be created automatically in the next step.

3. **Create and Update Database:**
   Run EF Core migrations to create the schema.

   ```bash
   dotnet ef database update
   ```

4. **Run the Application:**

   ```bash
   dotnet run
   ```

   The API will start on `http://localhost:5151`.
   Swagger UI is available at `http://localhost:5151/swagger`.

&nbsp;

## 📋 API Usage & Test Flow

For meaningful results, it’s recommended to run the endpoints in the following order.

### Suggested Test Flow

1. **Step 1: Generate Product Data**
   Create initial product data for reporting queries. The Dapper endpoint is significantly faster for this step.

   * `POST /dataimport/dapper-bulk-insert?count=10000`

2. **Step 2: Generate Orders**
   Create a large number of orders and order items linked to the products. This may take some time.

   * `POST /dataimport/generate-orders?orderCount=5000&maxItemsPerOrder=5`

3. **Step 3: Compare Reporting Endpoints**
   With enough data in place, run both reporting endpoints to observe performance differences.

   * `GET /reports/topsellers-efcore`
   * `GET /reports/topsellers-dapper`

### API Endpoints

All endpoints return execution time in milliseconds.

#### Data Generation Endpoints

| Method | Endpoint                         | Description                                                           |
| :----- | :------------------------------- | :-------------------------------------------------------------------- |
| `POST` | `/dataimport/efcore-bulk-insert` | Inserts the specified `count` of products using EF Core.              |
| `POST` | `/dataimport/dapper-bulk-insert` | Inserts the specified `count` of products using Dapper + SqlBulkCopy. |
| `POST` | `/dataimport/generate-orders`    | Generates random orders based on existing products.                   |

#### Reporting Endpoints

| Method | Endpoint                     | Description                                                   |
| :----- | :--------------------------- | :------------------------------------------------------------ |
| `GET`  | `/reports/topsellers-efcore` | Reports top 20 products by revenue using EF Core LINQ query.  |
| `GET`  | `/reports/topsellers-dapper` | Reports top 20 products by revenue using raw SQL with Dapper. |

&nbsp;

## 📊 Expected Results

When running the tests, you should expect the following:

* **Bulk Insert:** Dapper (with `SqlBulkCopy`) will be **significantly faster** than EF Core, because EF Core performs change tracking for every inserted entity, while Dapper writes directly to the database.

* **Complex Query:** Dapper will also be **faster** here, although the difference is less dramatic. Dapper avoids the LINQ-to-SQL translation and materialization overhead of EF Core.

&nbsp;

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).


&emsp;

# Dapper vs. EF Core Performans Karşılaştırma - TR

![.NET 8](https://img.shields.io/badge/.NET-8-blue.svg)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Bu proje, .NET ekosisteminin en popüler iki veritabanı erişim teknolojisi olan **Dapper** ve **Entity Framework Core**'un performansını karşılaştırmak için tasarlanmış bir REST API'ıdır.

Projenin temel amacı, geliştiricilere "kullanım kolaylığı" ile "ham performans" arasındaki dengeyi somut verilerle göstermektir.

## 📈 Karşılaştırılan Senaryolar

Bu benchmark projesi, ORM'lerin performans farklarının en net şekilde ortaya çıktığı iki kritik senaryoya odaklanır:

1.  **Toplu Veri Ekleme (Bulk Data Insertion):** Tek bir işlemde on binlerce kaydın veritabanına yazıldığı, yazma ağırlıklı (write-heavy) bir senaryodur. Bu test, EF Core'un change tracking (değişiklik izleme) mekanizmasının getirdiği ek yükü, Dapper'ın `SqlBulkCopy` ile entegre edilmiş ham performansına karşı ölçer.

2.  **Karmaşık Raporlama Sorgusu (Complex Read Query):** Birden fazla tablonun `JOIN` ile birleştirildiği, gruplama (`GROUP BY`) ve toplama (`SUM`) fonksiyonlarının kullanıldığı, okuma ağırlıklı (read-heavy) bir senaryodur. Bu test, EF Core'un LINQ sorgularını SQL'e çevirme maliyetini, Dapper ile çalıştırılan optimize edilmiş ham SQL sorgusunun hızına karşı ölçer.

&nbsp;

## 🛠️ Teknoloji

*   **Framework:** .NET 8
*   **API:** ASP.NET Core Web API
*   **Veritabanı:** SQL Server
*   **ORM / Micro-ORM:**
    *   Entity Framework Core
    *   Dapper
*   **Veri Üretimi:** Bogus (Gerçekçi sahte veri üretimi için)

&nbsp;

## Başlarken

Projeyi yerel makinenizde çalıştırmak ve test etmek için aşağıdaki adımları izleyin.

### Gereksinimler

*   [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
*   [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
*   [Git](https://git-scm.com/)

### Kurulum Adımları

1.  **Projeyi Klonlayın:**
    ```bash
    git clone https://github.com/kayamuhammet/Dapper-vs-EFCore.git
    cd backend
    ```

2.  **Veritabanı Bağlantısını Yapılandırın:**
    `appsettings.Development.json` dosyasını açın ve `ConnectionStrings` bölümünü kendi SQL Server örneğinizin bilgileriyle güncelleyin.
    ```json
    {
        "ConnectionStrings": {
            "DefaultConnection": "Server=(LocalServerName)\\SQLEXPRESS;Database=DbName;Trusted_Connection=True;TrustServerCertificate=True;"
        }
    }
    ```
    > **Not:** `Server` adını kendi sunucu adınızla değiştirmeyi unutmayın. Veritabanı, bir sonraki adımda otomatik olarak oluşturulacaktır.

3.  **Veritabanını Oluşturun ve Güncelleyin:**
    Proje kök dizininde bir terminal açın ve EF Core migration komutlarını çalıştırarak veritabanı şemasını oluşturun.
    ```bash
    dotnet ef database update
    ```

4.  **Uygulamayı Çalıştırın:**
    ```bash
    dotnet run
    ```
    Uygulama varsayılan olarak `http://localhost:5151` portlarında çalışmaya başlayacaktır. Swagger arayüzüne `http://localhost:5151/swagger` adresinden erişebilirsiniz.

&nbsp;

## 📋 API Kullanımı ve Test Akışı

En anlamlı sonuçları elde etmek için endpoint'leri aşağıdaki sırayla çalıştırmanız önerilir.

### Önerilen Test Akışı

1.  **Adım 1: Ürün Verilerini Oluşturun**
    İlk olarak, raporlama sorgularında kullanılacak temel ürün verilerini oluşturun. Dapper endpoint'i bu işlem için çok daha hızlıdır.
    *   `POST /dataimport/dapper-bulk-insert?count=10000`

2.  **Adım 2: Sipariş Verilerini Oluşturun**
    Oluşturulan ürünlere bağlı, yüksek sayıda sipariş ve sipariş kalemi verisi üretin. Bu işlem biraz zaman alabilir.
    *   `POST /dataimport/generate-orders?orderCount=5000&maxItemsPerOrder=5`

3.  **Adım 3: Raporlama Endpoint'lerini Karşılaştırın**
    Veritabanı artık yeterli veri içerdiğine göre, her iki raporlama endpoint'ini de çalıştırarak performans farkını gözlemleyebilirsiniz.
    *   `GET /reports/topsellers-efcore`
    *   `GET /reports/topsellers-dapper`

### API Endpoint'leri

Tüm endpoint'ler, işlemin ne kadar sürdüğünü milisaniye cinsinden gösteren bir yanıt döner.

#### Veri Oluşturma Endpoint'leri

| Method | Endpoint                                         | Açıklama                                                                |
| :----- | :----------------------------------------------- | :---------------------------------------------------------------------- |
| `POST` | `/dataimport/efcore-bulk-insert`                 | `count` parametresi kadar ürünü EF Core ile veritabanına ekler.         |
| `POST` | `/dataimport/dapper-bulk-insert`                 | `count` parametresi kadar ürünü Dapper ve `SqlBulkCopy` ile ekler.      |
| `POST` | `/dataimport/generate-orders`                    | Mevcut ürünleri kullanarak `orderCount` kadar rastgele sipariş oluşturur. |

#### Raporlama Endpoint'leri

| Method | Endpoint                       | Açıklama                                                             |
| :----- | :----------------------------- | :------------------------------------------------------------------- |
| `GET`  | `/reports/topsellers-efcore`   | En çok ciro yapan ilk 20 ürünü EF Core LINQ sorgusu ile raporlar.    |
| `GET`  | `/reports/topsellers-dapper`   | En çok ciro yapan ilk 20 ürünü Dapper ve ham SQL sorgusu ile raporlar. |

&nbsp;

## 📊 Beklenen Sonuçlar

Testleri çalıştırdığınızda aşağıdaki sonuçları gözlemlemeyi bekleyebilirsiniz:

*   **Toplu Ekleme:** Dapper (`SqlBulkCopy` kullanarak), EF Core'dan **önemli ölçüde** daha hızlı olacaktır. Bunun temel nedeni, Dapper'ın veriyi doğrudan veritabanına aktarması, EF Core'un ise eklediği her bir nesne için bir change tracking mekanizması çalıştırmasıdır.

*   **Karmaşık Sorgu:** Dapper, bu senaryoda da EF Core'dan **daha hızlı** olacaktır. Fark toplu eklemedeki kadar dramatik olmasa da, Dapper'ın doğrudan optimize edilmiş bir SQL sorgusunu çalıştırması, EF Core'un LINQ'yu SQL'e çevirme ve sonuçları nesnelere eşleme (materialization) adımlarından kaynaklanan ek maliyeti ortadan kaldırır.

&nbsp;

## Katkıda Bulunma

Katkıda bulunmak isterseniz, lütfen bir "issue" açın veya "pull request" gönderin.

## Lisans

Bu proje [MIT Lisansı](https://opensource.org/licenses/MIT) altında lisanslanmıştır.
