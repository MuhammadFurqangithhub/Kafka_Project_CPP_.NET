
# 📡 Kafka Integration: C++ Console App ↔ .NET Web API

This project demonstrates **bidirectional communication** between a **C++ Console Application** and a **.NET Core Web API** using **Apache Kafka**. Messages are sent and received in both directions via Kafka topics.

---

## 📁 Project Structure

```
KAFKA/
├── KafkaCppApp/             # Visual Studio C++ Console App
│   ├── KafkaCppApp.sln
│   └── KafkaCppApp/         # Contains KafkaCppApp.cpp
│
├── KafkaWebAPI/             # .NET 6 Web API
│   ├── KafkaWebAPI.csproj
│   └── Controllers/
```

---

## 🛠️ Prerequisites

### ✅ System Requirements

- **Windows 10/11**
- **Visual Studio (2019 or 2022)**
- **.NET Core SDK (>= 6.0)**
- **CMake + vcpkg**
- **Apache Kafka & Zookeeper**
- **Java JDK 11+** (for Kafka runtime)

---

## 🔧 Setup Instructions

### Step 1: Install Dependencies

#### 🧃 Java JDK 11

Install from [Adoptium.net](https://adoptium.net) or [Oracle](https://www.oracle.com/java/technologies/javase/jdk11-archive-downloads.html)

---

#### 🐘 Apache Kafka

Download and extract:

```bash
https://kafka.apache.org/downloads
```

Start Kafka:

```bash
# Start Zookeeper
bin/zookeeper-server-start.sh config/zookeeper.properties

# Start Kafka Broker
bin/kafka-server-start.sh config/server.properties
```

Create topics:

```bash
bin/kafka-topics.sh --create --topic cpp-to-web --bootstrap-server localhost:9092
bin/kafka-topics.sh --create --topic web-to-cpp --bootstrap-server localhost:9092
```

---

#### 🧰 Install `librdkafka` and `vcpkg`

```bash
git clone https://github.com/microsoft/vcpkg.git
cd vcpkg
bootstrap-vcpkg.bat
vcpkg install librdkafka:x64-windows
```

---

## 🚀 Build & Run

### ▶️ C++ Console App (Kafka Client)

1. Open project in **Visual Studio** or use **CMake**.
2. Link `librdkafka` using `vcpkg`.
3. Build and run:

```bash
# Sending message
./send_cpp_message.exe

# Receiving message
./receive_cpp_message.exe
```

---

### 🌐 .NET Core Web API

1. Open `dotnet-kafka-api` in Visual Studio or terminal.
2. Install NuGet package:

```bash
dotnet add package Confluent.Kafka
```

3. Run the API:

```bash
dotnet run
```

4. Test endpoints:

- `POST /api/send-to-cpp` → Sends Kafka message to C++ topic
- `GET /api/receive-from-cpp` → Polls messages from C++ sender

---

## 🔄 Communication Flow

```
C++ App (Producer) ──> [Kafka Topic: cpp-to-web] ──> Web API (Consumer)
Web API (Producer) ──> [Kafka Topic: web-to-cpp] ──> C++ App (Consumer)
```

---

## 🧪 Sample Topics Used

| Topic         | Producer       | Consumer       |
|---------------|----------------|----------------|
| `cpp-to-web`  | C++ App        | .NET Web API   |
| `web-to-cpp`  | .NET Web API   | C++ App        |

---

## 📂 Environment Notes

- All Kafka brokers and clients run on `localhost:9092`.
- For production, configure `bootstrap.servers`, security, and topic replication.
- Consider using **Docker Compose** for local Kafka + Zookeeper setup.

---

## 🔐 Security (Optional)

For SASL/SSL secured clusters, update configs in both:

- C++ (`rd_kafka_conf_set`)
- .NET (`ConsumerConfig`, `ProducerConfig`)

---

## ✨ References

- [Apache Kafka Documentation](https://kafka.apache.org/documentation/)
- [librdkafka GitHub](https://github.com/edenhill/librdkafka)
- [Confluent .NET Kafka Client](https://github.com/confluentinc/confluent-kafka-dotnet)

---

## 🧠 Author

**Muhammad Furqan**  
- 💼 Software Engineer 
- 🔗 GitHub: [@MuhammadFurqangithhub](https://github.com/MuhammadFurqangithhub)  
- ✍️ Medium: [@muhammadfurqan17](https://medium.com/@muhammadfurqan17)
