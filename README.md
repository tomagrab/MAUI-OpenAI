# Airole ✨

Welcome to Airole – your all-in-one AI-powered chat companion! With Airole, your conversations are more dynamic, creative, and engaging than ever before. Dive into a seamless blend of creativity and interaction, where you can:

- **Select Roles:** Choose unique roles for the AI to take on, from a cheerful elf to a wise philosopher, and experience personalized, immersive conversations.
- **Generate Appearances:** Bring your chats to life by generating vivid, custom images of the roles you're interacting with, creating a visually rich experience.
- **Speech:** Enable speech to listen to AI responses using various voices.
- **Voice Selection:** Select different voices for the AI, enhancing the auditory experience.

Whether you're sparking imagination, enhancing productivity, or just having fun, Airole turns everyday conversations into extraordinary adventures.

## Features 🌟

- **Role Selection:** Choose from a variety of roles for the AI to take on, such as a friendly elf, a wise philosopher, or a curious detective.
- **Appearance Generation:** Generate custom images of the roles you're interacting with, adding a visual element to your conversations.
- **Dynamic Conversations:** Engage in dynamic, creative conversations with the AI, tailored to the role you've selected.
- **Immersive Experience:** Immerse yourself in a unique, interactive experience that brings your chats to life.
- **Personalized Interactions:** Customize your interactions with the AI to suit your preferences and create a more engaging experience.
- **Speech:** Enable or disable speech to hear AI responses, making the interaction more lively.
- **Voice Selection:** Choose from various voices like Alloy, Echo, Fable, Onyx, Nova, and Shimmer for the AI to speak with.
- **Custom Tokenizer:** This project uses a custom tokenizer to process the input text before sending it to the OpenAI API.

## Custom Tokenizer 🧠

- **Purpose:** The custom tokenizer is designed to preprocess the input text before sending it to the OpenAI API.
- **Important Information:**
  - The custom tokenizer is implemented in the `TokenizerService.cs` file in the `Services` folder.
  - The custom tokenizer in this project is more conservative than the default OpenAI tokenizer.
    - OpenAI provides the following estimates for their tokenizer:
      - 1 token ≈ 4 characters in English
      - 1 token ≈ ¾ words
      - 100 tokens ≈ 75 words
    - The custom tokenizer in this project estimates 1 token ≈ 3.5 characters in English.
  - User and assistant messages are appended to a `Conversation` list, which is then tokenized and sent to the OpenAI API.
    - The `Conversation` list allows for a maximum of 80,000 tokens.
      - Once the conversation reaches 80,000 tokens, the oldest messages are removed to make room for new messages.
      - GPT-4o model allows for a maximum of 128,000 tokens in the Context Window.
        - The purpose of such conservative tokenization is to ensure that the conversation remains within the token limit, while the model can still retain context from previous messages.
  - Each user message allows for a maximum of 1000 tokens.
    - If the user message reaches 1000 tokens, further input is disabled.


## Tools Needed 🛠️

- Visual Studio 2022
- .NET 8
- OpenAI API Key

## Setup Instructions 🚀

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/tomagrab/MAUI-OpenAI.git
    ```
2. **Navigate to the Project Directory:**
    ```bash
    cd MAUI-OpenAI
    ```
3. **Restore Dependencies**
    ```bash
    dotnet restore
    ```
4. **Open the Project in Visual Studio 2022**

    Open Visual Studio 2022 and select `Open a project or solution`. Navigate to the project directory and select the `.sln` file.

5. **Add OpenAI API Key**
    - ***Windows 11: Edit system environment variables***
        - Search for `Edit the system environment variables` in the Start menu.
        - Click on `Environment Variables`.
        - Under `System variables`, click on `New`.
        - Add a new variable named `OPENAI_API_KEY` with your OpenAI API key as the value.
    - ***Linux***
        - Open the terminal
        - Temporarily set the API key using the following command:
            ```bash
            export OPENAI_API_KEY="your-api-key"
            ```
        - To permanently set the API key, add the following line to your `.bashrc` or `.bash_profile` file:
            ```bash
            echo 'export OPENAI_API_KEY=your_openai_api_key_here' >> ~/.bashrc
            ```
            or
            ```bash
            echo 'export OPENAI_API_KEY=your_openai_api_key_here' >> ~/.bash_profile
            ```
        - ZSH users can add the line to the `.zshrc` file.
          ```bash
          echo 'export OPENAI_API_KEY=your_openai_api_key_here' >> ~/.zshrc
          ```
      - Reload the configuration file:
          ```bash
          source ~/.bashrc
          ```
          or
          ```bash
          source ~/.bash_profile
          ```
          or
          ```bash
          source ~/.zshrc
          ```
    - ***macOS***
      - Open the terminal
      - Temporarily set the API key using the following command:
          ```bash
          export OPENAI_API_KEY="your-api-key"
          ```
      - To permanently set the API key, add the following line to your `.bashrc` or `.bash_profile` or `.zshrc` file:
          ```bash
          echo 'export OPENAI_API_KEY=your_openai_api_key_here' >> ~/.bash_profile
          ```
          or
          ```bash
          echo 'export OPENAI_API_KEY=your_openai_api_key_here' >> ~/.bashrc
          ```
          or
          ```bash
          echo 'export OPENAI_API_KEY=your_openai_api_key_here' >> ~/.zshrc
          ```
      - Reload the configuration file:
          ```bash
          source ~/.bash_profile
          ```
          or
          ```bash
          source ~/.bashrc
          ```
          or
          ```bash
          source ~/.zshrc
          ```

## Run the App 🎉

1. **Select the Target Platform**

    In Visual Studio 2022, select the target platform you want to run the app on. You can choose from:
    - Android Emulator (requires Android SDK) 📱
    - iOS Simulator (requires Xcode) 🍏
    - Windows (WinUI) 🖥️
    - macOS (Catalyst) 💻

2. **Run the App**

    Click on the `Run` button in Visual Studio 2022 to build and run the app on the selected platform.

## Open Source 🌍

Airole is an open-source project. We welcome contributions and feedback from the community. Feel free to fork the repository, submit pull requests, or open issues to help improve the app.

## License 📄


Airole is licensed under the MIT License. See the [License](https://github.com/tomagrab/MAUI-OpenAI/blob/main/License.md) file for more information.


## Acknowledgements 🙏

We would like to thank the following resources for their valuable contributions to this project:
   - OpenAI API
   - .NET MAUI Community
   - Visual Studio 2022