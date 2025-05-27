#include <iostream>
#include <librdkafka/rdkafkacpp.h>
#include <thread>
#include <vector>
#include <string>

// Kafka broker and topics
const std::string brokers = "localhost:9092";
const std::string topic_send = "cppweb";   // Producer's sending topic
const std::string topic_receive = "cppweb"; // Consumer's receiving topic
const std::string group_id = "cpp-consumer-group2";

// Function to send a message from C++ to Kafka (Producer)
void sendMessage(const std::string& msg) {
    std::string errstr;

    // Manually create the JSON string (you can customize this format)
    std::string json_msg = "{\"message\": \"" + msg + "\"}";  // Construct a JSON-like string

    // Configure the producer
    RdKafka::Conf* conf = RdKafka::Conf::create(RdKafka::Conf::CONF_GLOBAL);
    conf->set("bootstrap.servers", brokers, errstr);
    conf->set("acks", "all", errstr);

    // Create the producer instance
    RdKafka::Producer* producer = RdKafka::Producer::create(conf, errstr);
    if (!producer) {
        std::cerr << "Failed to create producer: " << errstr << std::endl;
        delete conf;
        return;
    }

    // Create topic object
    RdKafka::Topic* topic = RdKafka::Topic::create(producer, topic_send, nullptr, errstr);
    if (!topic) {
        std::cerr << "Failed to create topic: " << errstr << std::endl;
        delete producer;
        delete conf;
        return;
    }

    // Produce message to the Kafka topic
    const std::string* key = nullptr;  // Optional: use key if needed

    RdKafka::ErrorCode resp = producer->produce(
        topic,                                 // ✅ Topic pointer, not string
        RdKafka::Topic::PARTITION_UA,          // Partition
        RdKafka::Producer::RK_MSG_COPY,        // Copy payload
        const_cast<char*>(json_msg.c_str()),   // Payload in JSON-like format
        json_msg.size(),                      // Payload size
        key,                                   // Optional key
        nullptr                                // Optional opaque pointer
    );

    if (resp != RdKafka::ERR_NO_ERROR) {
        std::cerr << "Failed to produce message: " << RdKafka::err2str(resp) << std::endl;
    }
    else {
        std::cout << "[Kafka Sent]" << json_msg << std::endl;
    }

    // Wait for delivery
    producer->flush(1000);

    // Clean up
    delete topic;
    delete producer;
    delete conf;
}

// Function to receive messages from Kafka (Consumer)
// Function to receive messages from Kafka (Consumer)
void receiveMessages() {
    std::string errstr, extracted_msg;

    // Configure the consumer
    RdKafka::Conf* conf = RdKafka::Conf::create(RdKafka::Conf::CONF_GLOBAL);
    conf->set("bootstrap.servers", brokers, errstr);
    conf->set("group.id", "cpp-consumer-group", errstr);
    conf->set("auto.offset.reset", "earliest", errstr);  // Read from the beginning if no offset

    // Create the consumer instance
    RdKafka::KafkaConsumer* consumer = RdKafka::KafkaConsumer::create(conf, errstr);
    if (!consumer) {
        std::cerr << "Failed to create KafkaConsumer: " << errstr << std::endl;
        delete conf;
        return;
    }

    // Subscribe to the topic
    std::vector<std::string> topics = { topic_send };
    RdKafka::ErrorCode err = consumer->subscribe(topics);
    if (err != RdKafka::ERR_NO_ERROR) {
        std::cerr << "Failed to subscribe: " << RdKafka::err2str(err) << std::endl;
        delete consumer;
        delete conf;
        return;
    }

    // Start consuming messages
    while (true) {
        RdKafka::Message* msg = consumer->consume(5000); // Wait for 5 seconds
        if (msg) {
            if (msg->err() == RdKafka::ERR_NO_ERROR) {
                // Parse and extract the message content (ignoring the JSON structure)
                std::string received_msg = static_cast<const char*>(msg->payload());
                size_t start_pos = received_msg.find("\"message\": \"") + 12;
                size_t end_pos = received_msg.find("\"}", start_pos);

                if(received_msg[start_pos] != '"')
                    extracted_msg = received_msg.substr(start_pos, end_pos - start_pos);
                else
                    extracted_msg = received_msg.substr(start_pos+1, end_pos - start_pos-1);

                std::cout << "[Kafka Received] " << extracted_msg << std::endl;
            }
            else if (msg->err() == RdKafka::ERR__TIMED_OUT) {
                std::cout << "[INFO] Timeout - No new messages\n";
            }
            else {
                std::cerr << "Consumer error: " << msg->errstr() << std::endl;
            }

            delete msg;
        }
    }

    // Cleanup
    consumer->close();
    delete consumer;
    delete conf;
}


// Main function
int main() {
    std::string choice;
    std::cout << "Enter 'send' to send a message, 'receive' to consume messages: ";
    std::cin >> choice;
    std::cin.ignore();  // To ignore the newline after user input

    if (choice == "send") {
        std::string message;
        while (true) {
            std::cout << "Enter message to send to Kafka: ";
            std::getline(std::cin, message);
            if (message == "exit") break;  // Exit if user types "exit"

            sendMessage(message);
        }
    }
    else if (choice == "receive") {
        std::cout << "Now in consumer mode...\n";
        receiveMessages();
    }
    else {
        std::cout << "Invalid choice\n";
    }

    return 0;
}
