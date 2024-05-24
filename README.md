# FlashCardConsoleApp
https://www.thecsharpacademy.com/project/12/flash-card-logger

This is a console application that allows users to create and manage flashcards for study purposes. The application uses a SQLite database to store and retrieve flashcard data. Users can insert, delete, update, and view their flashcards. The application also provides a study mode to review flashcards.

## Features

- SQLite Database: The application creates a SQLite database if one doesn't exist and creates tables to store flashcard data, stack data, and study session data. This is handled by the `FlashCardDbContext` class.
- Stack Management: Users can create, update, and delete stacks of flashcards. This is managed by the `StackService` class.
- Flashcard Management: Users can add flashcards to a stack, update flashcards, and delete flashcards from a stack. This is also managed by the `StackService` class.
- Study Sessions: Users can study a stack of flashcards and record their study session data, including the date, correct count, and incorrect count. This is implemented in the `Application` class and managed by the `StackService` class.
- Menu Options: The application presents a menu to the user with options to manage stacks and flashcards, study a stack, view study session data, and exit the application. This is implemented using the `SelectionPrompt` class from the `Spectre.Console` library.
- Error Handling: The application handles possible errors to ensure it doesn't crash and provides appropriate error messages to the user.
- Termination: The application continues to run until the user chooses the "Exit" option.
- Demo Stacks: The application can read in example stacks from a JSON file and generate example study session data.
- Unicode Support: The application supports Unicode characters. To display these characters correctly in the console, you may need to change your console settings.

## Getting Started

To run the application, follow these steps:

1. The user has to configure the app.config file with the appropiate connection string for SQL Sever
2. Make sure you have the necessary dependencies installed, including Microsoft.Data.Sqlite, Spectre.Console, and Microsoft.EntityFrameworkCore.
3. Clone the repository to your local machine.
4. Open the solution in Visual Studio.
5. Build the solution to restore NuGet packages and compile the code.
6. Run the application.

## Dependencies

- Microsoft.Data.Sqlite: The application uses this package to interact with the SQLite database.
- Microsoft.EntityFrameworkCore: The application uses this package to manage the database context and entity relationships.
- Spectre.Console: The application uses this package to create a user-friendly console interface.

## Usage

1. When the application starts, it will create a SQLite database if one doesn't exist and create tables to store flashcard data, stack data, and study session data.
2. If there are no stacks in the database, the application will offer read in example stacks from a JSON file and generate example study session data.
3. The application will display a menu with options to manage stacks and flashcards, study a stack, view study session data, or exit the application.		
4. Select an option by using the arrow keys and press Enter
5. Follow the prompts to perform the desired action.
6. The application will continue to run until you choose the "Exit" option.

## Unicode Support

To get the '?' characters to display, right click on the console window menu bar -> properties -> change the font to one that supports unicode like NSimSum 

## License

This project is licensed under the [MIT License](LICENSE).

## Resources Used
- [The C# Academy](https://www.thecsharpacademy.com/project/12/flash-card-logger)
- GitHub Copilot to generate code snippets