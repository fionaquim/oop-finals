using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;


namespace LibraryFiona
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string filePath = "\"C:\\Users\\ZachyBoi\\source\\repos\\LibraryFiona\\LibraryFiona\\default_books.txt\"";
        private List<Book> books = new List<Book>();
        private int currentIndex = -1; // (initialized to -1, meaning no book is displayed initially).

        public MainWindow()
        {
            InitializeComponent();

            // Define the default file path
            filePath = @"C:\Users\ZachyBoi\source\repos\LibraryFiona\LibraryFiona\default_books.txt"; // Update with your actual file path

            // Check if the file exists
            if (File.Exists(filePath))
            {
                LoadBookData(); // Load the file automatically
            }
            else
            {
                MessageBox.Show($"File not found: {filePath}. Please place the file in the correct location.", "File Not Found");
            }
        }


        private void LoadBookData()
        {
            try
            {
                books.Clear(); // ensures that the books list is emptied before loading new data

                // Use StreamReader to read the file line by line
                using (StreamReader reader = new StreamReader(filePath))
                {
                    // ReadLine() method of StreamReader reads one line of the file at a time.
                    // The loop continues until there are no more lines to read (ReadLine() returns null when the end of the file is reached).
                    // Each line is assigned to the line variable.
                    string line; // will be used to store each line of text that is read from the file
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split('|'); // splits the string line into an array of substrings
                        if (parts.Length == 5)
                        {
                            books.Add(new Book // The Add() method is called on the books list to add the newly created Book object to the list.
                            {
                                Title = parts[0],
                                Author = parts[1],
                                PublishedDate = parts[2],
                                ISBN = parts[3],
                                Genre = parts[4]
                            });
                        }
                    }
                }

                if (books.Count > 0) //This line checks if there are any books in the books list after the file has been processed.                    
                {
                    PopulateComboBoxes(); // Populate filters
                    currentIndex = 0;     // Start with the first book
                    PopulateForm(books[currentIndex]); // Display the first book
                }
                else
                {
                    MessageBox.Show("No books found in the file.", "Info");
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file: {ex.Message}", "Error");
            }
        }


        private void PopulateForm(Book book)
        {
            if (book != null) // if book not equal to = null
            // If the book is null, the code inside the if block won't be executed
            {
                tbTITLE.Text = book.Title;
                Cb_Author.Text = book.Author;
                tbPD.Text = book.PublishedDate;
                tbISBN.Text = book.ISBN;
                Cb_Genre.Text = book.Genre;
            }
        }
        /// Select(b => b.Author) to get the Author property of each book.
        /// Distinct() removes duplicate authors, ensuring each author appears only once.
        /// OrderBy(a => a) sorts the authors alphabetically in ascending order. <summary>
        /// Select(b => b.Author) to get the Author property of each book.
        /// ToList() converts the sorted authors into a list that can be used by the ComboBox.
        private void PopulateComboBoxes()
        {
            // Get distinct authors from the books list,then order them alphabetically, and convert to a list
            var uniqueAuthors = books.Select(b => b.Author).Distinct().OrderBy(a => a).ToList();
            var uniqueGenres = books.Select(b => b.Genre).Distinct().OrderBy(g => g).ToList();

            // Assign the updated lists to the ComboBoxes
            Cb_Author.ItemsSource = uniqueAuthors;
            Cb_Genre.ItemsSource = uniqueGenres;
        }

        public class Book // blueprint for representing a book
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Author { get; set; }
            public string PublishedDate { get; set; }
            public string ISBN { get; set; }
            public string Genre { get; set; }
        }

        //When the user selects an author in the Cb_Author ComboBox, this method is triggered.
        //It gets the selected author as a string (selectedAuthor).
        //If a valid author is selected:
        //It filters the books list to find books written by that author.
        //It passes the filtered list to DisplayFilteredBooks to update the UI with the results.
        //If no author is selected or the selection is invalid (e.g., null or empty), nothing happens.
        private void Cb_Author_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedAuthor = Cb_Author.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedAuthor))
            {
                var filteredBooks = books.Where(b => b.Author == selectedAuthor).ToList();
                DisplayFilteredBooks(filteredBooks);
            }
        }
        // //When the user selects a genre in the Cb_Genre ComboBox, this method is triggered automatically.
        //The method retrieves the selected genre(selectedGenre) as a string.
        //If the genre is valid(not null or empty) :
        //The books list is filtered to include only the books with the matching Genre.
        //The filtered list (filteredBooks) is passed to the DisplayFilteredBooks method,
        //which updates the UI to display the matching books.
        //If no genre is selected or the selected genre is invalid, nothing happens.
        private void Cb_Genre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedGenre = Cb_Genre.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedGenre))
            {
                var filteredBooks = books.Where(b => b.Genre == selectedGenre).ToList();
                DisplayFilteredBooks(filteredBooks);
            }
        }

        // Display the first book from the filtered list or show a message if none match
        private void DisplayFilteredBooks(List<Book> filteredBooks)
        {
            if (filteredBooks.Any()) // if contains any books
            {
                PopulateForm(filteredBooks.First());
            }
            else
            {
                MessageBox.Show("No books found for the selected criteria.");
                ClearForm();
            }
        }

        private void BTN_ADD_Click(object sender, RoutedEventArgs e)
        {
            // Ensure all fields are filled
            if (string.IsNullOrWhiteSpace(tbTITLE.Text) || string.IsNullOrWhiteSpace(Cb_Author.Text) ||
                string.IsNullOrWhiteSpace(tbPD.Text) || string.IsNullOrWhiteSpace(tbISBN.Text) ||
                string.IsNullOrWhiteSpace(Cb_Genre.Text))
            {
                MessageBox.Show("All fields must be filled out!", "Validation Error");
                return;
            }

            // Check if the book already exists by ISBN
            if (books.Any(b => b.ISBN == tbISBN.Text))
            {
                MessageBox.Show("A book with this ISBN already exists.", "Duplicate Entry");
                return;
            }

            // Create a new book object from the input fields
            Book newBook = new Book
            {
                Title = tbTITLE.Text,
                Author = Cb_Author.Text,
                PublishedDate = tbPD.Text,
                ISBN = tbISBN.Text,
                Genre = Cb_Genre.Text
            };

            // Add the new book to the in-memory list and save
            books.Add(newBook);
            SaveBooksToFile();
            LoadBookData();
            MessageBox.Show("Book added successfully!");
            ClearForm(); // Clear the input form after adding
        }


        private void SaveBooksToFile()
        {
            try
            {
                // Convert each book object to a line of text using '|' as a delimiter
                var lines = books.Select(b => $"{b.Title}|{b.Author}|{b.PublishedDate}|{b.ISBN}|{b.Genre}");
                File.WriteAllLines(filePath, lines); // Overwrite the file with updated data
            }
            catch (Exception ex)
            {
                // Display an error message if saving fails
                MessageBox.Show("Error saving file: " + ex.Message, "Error");
            }
        }

        private void BTN_UPDATE_Click(object sender, RoutedEventArgs e)
        {
            // Ensure all fields are filled
            if (string.IsNullOrWhiteSpace(tbTITLE.Text) || string.IsNullOrWhiteSpace(Cb_Author.Text) ||
                string.IsNullOrWhiteSpace(tbPD.Text) || string.IsNullOrWhiteSpace(Cb_Genre.Text))
            {
                MessageBox.Show("All fields must be filled out to update the book!", "Validation Error");
                return;
            }

            // Find the book to update using the Title (unique identifier)
            Book bookToUpdate = books.FirstOrDefault(b => b.Title == tbTITLE.Text);

            if (bookToUpdate != null) // checks if book was found
            {
                // Confirm update
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to update the book \"{bookToUpdate.Title}\"?",
                    "Confirm Update",
                    MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    // Update the book's details
                    bookToUpdate.Author = Cb_Author.Text;
                    bookToUpdate.PublishedDate = tbPD.Text;
                    bookToUpdate.Genre = Cb_Genre.Text;

                    // Save the updated list to the file
                    SaveBooksToFile();
                    LoadBookData();            // Reload the updated data
                    PopulateComboBoxes();      // Update the ComboBoxes
                    MessageBox.Show("Book updated successfully!");
                }
            }
            else
            {
                MessageBox.Show("Book not found. Please ensure the correct title is entered.", "Error");
            }
        }


        private void BTN_DELETE_Click(object sender, RoutedEventArgs e)
        {
            // Ensure a book is selected
            if (string.IsNullOrWhiteSpace(tbISBN.Text))
            {
                MessageBox.Show("No book selected to delete.", "Delete Error");
                return;
            }

            // Find the book to delete using the ISBN (unique identifier)
            Book bookToDelete = books.FirstOrDefault(b => b.ISBN == tbISBN.Text);

            if (bookToDelete != null)
            {
                // Confirm deletion
               MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete the book \"{bookToDelete.Title}\"?", "Confirm Deletion", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    books.Remove(bookToDelete); // Remove the book from the list
                    SaveBooksToFile();         // Save the updated list to the file
                    LoadBookData();            // Reload the updated data
                    PopulateComboBoxes();      // Update the ComboBoxes
                    ClearForm();               // Clear the form
                    currentIndex = -1;         // Reset the index
                    MessageBox.Show("Book deleted successfully!");
                }
            }
            else
            {
                MessageBox.Show("Book not found in the list. Please try again.", "Error");
            }
        }

        private void ClearForm()
        {
            tbTITLE.Text = "";
            Cb_Author.SelectedIndex = -1;
            tbPD.Text = "";
            tbISBN.Text = "";
            Cb_Genre.SelectedIndex = -1;

        }

        private void BTN_CLEAR_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }
    }
}
