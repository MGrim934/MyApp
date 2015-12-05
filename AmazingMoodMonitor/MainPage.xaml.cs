using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AmazingMoodMonitor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        List <Mood> _moodList;
        List<Activity> _activityList;
        ArrayList _previousEntries = new ArrayList();
        Random roll = new Random();
        ArrayList moodDupCheck = new ArrayList();
        ArrayList actDupCheck = new ArrayList();
        ArrayList _moreLikeThisList = new ArrayList();
        Boolean bookOpen = false;

        string mood, activity;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
           
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //if the data is already loaded then exit the method
            if (_moodList != null && _activityList != null)
            {
                generateMoodGrid();
                generateActivityGrid();
                return;
            }

            _moodList = new List<Mood>();
            _activityList = new List<Activity>();
            loadLocalData();
            lvJournal.ItemsSource = _previousEntries;




            // lvMoods.ItemsSource = _moodList;


        }



        private async void loadLocalData()
        {
            var moodsFile = await Package.Current.InstalledLocation.GetFileAsync("Data\\moods.txt");
            var fileText = await FileIO.ReadTextAsync(moodsFile);


            try
            {
                var moodsJArray = JsonArray.Parse(fileText);
                createListofMoods(moodsJArray);
               
            }
            catch (Exception exJA)
            {
                MessageDialog dialog = new MessageDialog(exJA.Message);
                await dialog.ShowAsync();

            }


            var activitiesFile = await Package.Current.InstalledLocation.GetFileAsync("Data\\activities.txt");
            fileText = await FileIO.ReadTextAsync(activitiesFile);


            try
            {
                var activitiesJArray = JsonArray.Parse(fileText);
                createListofActivities(activitiesJArray);

            }
            catch (Exception exJA)
            {
                MessageDialog dialog = new MessageDialog(exJA.Message);
                await dialog.ShowAsync();

            }

            generateMoodGrid();
            generateActivityGrid();
            ReadJournalFile();


        }//load data

        private void createListofMoods(JsonArray moodsJArray)
        {
            foreach (var item in moodsJArray)
            {
                // get the object
                var obj = item.GetObject();

               
                Mood mood = new Mood();

                // get each key value pair and sort it to the appropriate elements
                // of the class
                foreach (var key in obj.Keys)
                {
                    IJsonValue value;
                    if (!obj.TryGetValue(key, out value))
                        continue;

                    switch (key)
                    {
                        case "mood":
                            mood.name = value.GetString();
                            break;
                        case "image":
                           mood.image = value.GetString();
                            break;
                       
                    }
                } // end foreach (var key in obj.Keys)
                _moodList.Add(mood);
            } // end foreach (var item in array)
        }//createList of Moods



        private void createListofActivities(JsonArray actJArray)
        {
            foreach (var item in actJArray)
            {
                // get the object
                var obj = item.GetObject();


                Activity activity = new Activity();

                // get each key value pair and sort it to the appropriate elements
                // of the class
                foreach (var key in obj.Keys)
                {
                    IJsonValue value;
                    if (!obj.TryGetValue(key, out value))
                        continue;

                    switch (key)
                    {
                        case "activity":
                            activity.name = value.GetString();
                            break;
                        case "image":
                           activity.image = value.GetString();
                            break;

                    }
                } // end foreach (var key in obj.Keys)
                _activityList.Add(activity);
            } // end foreach (var item in array)
        }//createList of Moods


        private  void generateMoodGrid()
        {
            Image img;
            int moodChoice;
            //fill up grid
            for(int i = 0; i < 5; i++)
            {
                for(int j = 0; j < 2; j++)
                {
                    img = new Image();

                    do
                    {
                       moodChoice = roll.Next(0, (_moodList.Count));
                    } while (moodDupCheck.Contains(moodChoice));


                    img.Name = _moodList[moodChoice].name;
                    moodDupCheck.Add(moodChoice);
                    string thing = "ms-appx://"+ _moodList[moodChoice].image;

                    Uri pic = new Uri(thing);
                    ImageBrush fill = new ImageBrush();
                   fill.ImageSource = new BitmapImage(pic);
                    img.Source = fill.ImageSource;
                    img.Width = 75;
                    img.Height = 75;
                    

                    img.SetValue(Grid.RowProperty, i);
                    img.SetValue(Grid.ColumnProperty, j);
                    img.Tapped += Mood_Tapped;
                    gdMood.Children.Add(img);

                }
            }
        }

        private void Mood_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image curr = (Image)sender;
            //change text
            //show other grid
            gdMood.Visibility = Visibility.Collapsed;

            gdActivity.Visibility = Visibility.Visible;
            //get mood type
            //get activity type
            //get date
            //write to file
            JournalEntry entry = new JournalEntry();
            mood = curr.Name;

            



        }



        private  void generateActivityGrid()
        {
            Image img;
            int activityChoice;
            //fill up grid
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    img = new Image();

                    do
                    {
                       activityChoice = roll.Next(0, (_activityList.Count));
                    } while (actDupCheck.Contains(activityChoice));


                    img.Name = _activityList[activityChoice].name;
                   actDupCheck.Add(activityChoice);
                    string thing = "ms-appx://" + _activityList[activityChoice].image;

                    Uri pic = new Uri(thing);
                    ImageBrush fill = new ImageBrush();
                    fill.ImageSource = new BitmapImage(pic);
                    img.Source = fill.ImageSource;
                    img.Width = 100;
                    img.Height = 75;


                    img.SetValue(Grid.RowProperty, i);
                    img.SetValue(Grid.ColumnProperty, j);
                    img.Tapped +=Activity_Tapped;
                    gdActivity.Children.Add(img);

                }
            }
        }//activitygrid

        private void Activity_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gdMood.Visibility = Visibility.Visible;
            gdActivity.Visibility = Visibility.Collapsed;

            Image curr = (Image)sender;
            activity = curr.Name;


            createJournalEntry();
        }



        private void createJournalEntry()
        {
            JournalEntry entry = new JournalEntry();
            int index = _moodList.FindIndex(item => item.name.Equals(mood)); 
           
            entry.mood =_moodList[index].image;

            index = _activityList.FindIndex(item => item.name.Equals(activity));

            entry.activity = _activityList[index].image;
            
          
            entry.dateTime = DateTime.Now;
            //add to list
            _previousEntries.Insert(0,entry); //adds it to the top


            JournalEntry(entry);//adds journal to entry file for storage


            updateJournal();
            ptMain.SelectedIndex = 2; ;
           


        }

        private void tbNewEntry_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //change selected index
            ptMain.SelectedIndex = 1;
            //opens up journal
            changeBackground();
           
            


        }

        private void tbPrevious_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ptMain.SelectedIndex = 2;


            changeBackground();
           
        }

        private void changeBackground()
        {
            Uri pic = new Uri("ms-appx:///Images/bg.jpg",UriKind.RelativeOrAbsolute);
            ImageBrush fill = new ImageBrush();
            fill.ImageSource = new BitmapImage(pic);

            

            gdMaster.Background = fill;
            bookOpen = true;
        }

        private void closeBook(object sender, TappedRoutedEventArgs e)
        {
            Pivot curr = (Pivot)sender;

            if (curr.SelectedIndex == 0) { 
            Uri pic = new Uri("ms-appx:///Images/main.png", UriKind.RelativeOrAbsolute);
            ImageBrush fill = new ImageBrush();
            fill.ImageSource = new BitmapImage(pic);
                bookOpen = false;
                

            gdMaster.Background = fill;
            }
            else if(bookOpen!=true)
            {
                changeBackground();
                
            }
        
        }

        private void updateJournal()
        {
  
            lvJournal.ItemsSource = null;
            lvJournal.ItemsSource = _previousEntries;
            

            //open file or create file if there is none
            //check  mood
            //if there then give it the location of the image
            //activity check give image
            //read in date and time
            //update previous entries
        }

        async void ReadJournalFile()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile journalFile;

            try
            {
                journalFile = await storageFolder.GetFileAsync("journal.txt");
            }
            catch (Exception myE)
            {
                string message = myE.Message;
                return;
            }

            string fileText=await Windows.Storage.FileIO.ReadTextAsync(journalFile);

            //run method that parses this string and adds elements to the array from it
            tbTest.Text = fileText;
            AddToJournal(fileText);

        }

         void AddToJournal(String text)
        {
            string[] words = text.Split(',','\n');

            for(int i = 0; i < words.Length; i+=3)
            {
                //create a new entry
                if ((i + 3) < words.Length)
                {
                    JournalEntry entry = new JournalEntry();
                    entry.mood = words[i];
                    entry.activity = words[i + 1];


                    string date = words[i + 2];
                    entry.dateTime = Convert.ToDateTime(date);
                    _previousEntries.Add(entry);
                }
                
            }
            _previousEntries.Reverse();


        }

        //write to journal
        async void JournalEntry(JournalEntry entry)
        {

            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            // create the file and append
            StorageFile journalFile;
            string fileText = ",";
            try
            {
                journalFile = await storageFolder.GetFileAsync("journal.txt");
                fileText = await Windows.Storage.FileIO.ReadTextAsync(journalFile);

            }
            catch (Exception myE)
            {
                string message = myE.Message;
                journalFile = await storageFolder.CreateFileAsync("journal.txt");
            }
            string jLine;
            jLine = entry.mood + "," + entry.activity + "," + entry.dateTime;
            await Windows.Storage.FileIO.WriteTextAsync(journalFile, fileText + jLine + System.Environment.NewLine);
            //test
            tbTest.Text= tbTest.Text + jLine + System.Environment.NewLine;

        }

        //if the user clicks to clear the journal
        private void btnClearQ_Tapped(object sender, TappedRoutedEventArgs e)
        {
            btnClearQ.Content = "Are you Sure ?"; //gotta make sure
            btnClearQ.Tapped -= btnClearQ_Tapped; //stop them from double tapping
            //make other buttons visible
            btnClearY.Visibility = Visibility.Visible;
            btnClearN.Visibility = Visibility.Visible;
        }

        private void btnClearYN_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Button current = (Button)sender;
            //check index
            char index = current.Name[8];

            switch (index)
            {
                case 'Y':
                    clearJournal();
                    clearJournalArray();
                  
                    break;
                case 'N':
                    //don't clear the journal!
                    break;
             
            }
            resetButtons();




        }
        void resetButtons()
        {
            btnClearQ.Content = "Clear Previous Entries";
            btnClearQ.Tapped += btnClearQ_Tapped;

            btnClearY.Visibility = Visibility.Collapsed;
            btnClearN.Visibility = Visibility.Collapsed;
        }

        private void clearJournalArray()
        {
            _previousEntries.Clear();
            updateJournal();
            
        }

        private void lvJournal_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //get item at selected index
           PivotlimitCheck();




            ListView current = (ListView) sender;
            //find current entry
            if (_previousEntries.Count<1)
            {
                return; //breaks out of this method if there are no entries
            }
            JournalEntry entry =(JournalEntry) _previousEntries[lvJournal.SelectedIndex];
            PivotItem piNew = new PivotItem();
            piNew.Name = "piStat";
            piNew.Header ="Statistics";
            StackPanel stk = new StackPanel();
            TextBlock block = new TextBlock();
            TextBlock header = new TextBlock();
            stk.Orientation = Orientation.Horizontal;
            
            //add images and text
            Image mood = new Image();

            mood.Width =75;
            mood.Height =75;
            
            mood.Source = new BitmapImage (new Uri("ms-appx://"+entry.mood, UriKind.RelativeOrAbsolute));
            mood.Name = entry.mood;
            mood.Tapped += more_Tapped;
            Image activity = new Image();
            activity.Width =100;
            activity.Height = 75;
            activity.Name = entry.activity;
            activity.Source = new BitmapImage(new Uri("ms-appx://" + entry.activity, UriKind.RelativeOrAbsolute));

            TextBlock date = new TextBlock();
            date.TextAlignment = TextAlignment.Center;
            date.Text = entry.dateTime.ToString();
            date.Margin = new Thickness(20, 0, 0, 0);
            stk.Children.Add(mood);
            stk.Children.Add(activity);
            stk.Children.Add(date);

            header.Text = "More Like This?";
            header.Style = (Style) Resources["HeaderTextBlockStyle"];
            header.Margin = new Thickness(0, 0, 0, 40);
            header.TextAlignment = TextAlignment.Center;

            TextBlock question = new TextBlock();
            question.Text = "?";
           question.Style = (Style)Resources["HeaderTextBlockStyle"];
            question.FontSize = 150;
            question.Margin = new Thickness(0, 20, 0, 0);
            question.TextAlignment = TextAlignment.Center;

            StackPanel stack = new StackPanel();
            stack.Children.Add(header);

            stack.HorizontalAlignment=HorizontalAlignment.Center;
            stack.Children.Add(stk);
            stack.Children.Add(question);
            
           
        
         
      
         
           
            piNew.Content = stack;
 
            ptMain.Items.Add(piNew);
            ptMain.SelectedIndex=4;
          

            //offer user choice
            //more like this
            //mood
            //activity


        }

        private void PivotlimitCheck()
        {

            if (ptMain.Items.Count > 4)
                ptMain.Items.RemoveAt(4);

        }

        private void updateList(ListView lvMoreLikeThis)
        {
            lvMoreLikeThis.ItemsSource = null;
            lvMoreLikeThis.ItemsSource = _moreLikeThisList;
        }

        private void more_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PivotlimitCheck(); //removes stats page if theres already one there
            //stops the user from generating many pages
          
            Image img = (Image)sender; //cast the sender
            fillList(img); //files the list with the mood associated with the image
            //remove old pivot page
            //create new pivot page
            PivotItem piNew = new PivotItem(); //creates a new pivot item
            piNew.Name = "piStat";
            piNew.Header = "Statistics"; //header
            StackPanel stk = new StackPanel();
            stk.Orientation = Orientation.Horizontal;

            StackPanel stack = new StackPanel();
            TextBlock sim = new TextBlock();
            sim.Text = "Similar Entries";
            sim.Style = (Style)Resources["HeaderTextBlockStyle"];
            sim.Margin = new Thickness(0, 0, 0, 40);
            sim.TextAlignment = TextAlignment.Center;

            stack.HorizontalAlignment = HorizontalAlignment.Center;
            stack.Orientation = Orientation.Vertical;
          


            ListView lvMoreLikeThis = new ListView(); //creates a new list view
            lvMoreLikeThis.ItemsSource = _moreLikeThisList; //sets the source
            updateList(lvMoreLikeThis);
            lvMoreLikeThis.Name = "lvMoreLikeThis";
            lvMoreLikeThis.ItemTemplate = journalEntryTemp; //sets the item template that the other list view uses
            stack.Children.Add(sim);
            stack.Children.Add(lvMoreLikeThis); //adds it to the stackpanel
            piNew.Content = stack; //sets this pivot items content as the stackpanel
            ptMain.Items.Add(piNew);//adds a new stack panel
            ptMain.SelectedIndex=4; //moves to the new page that was generated




        }


        private void fillList(Image img)
        {
            
         
            //check against _previous entries
            //fill new arraylist
            //create listview
            //send to list view
            JournalEntry entry; //creates a new journal entry for the list
            _moreLikeThisList.Clear();
            for (int i = 0; i < _previousEntries.Count; i++) //checks every element of the array
            {
                entry = (JournalEntry)_previousEntries[i];
                if (entry.mood.Equals(img.Name)) //if we've seen another instance of this mood, add it to the list. Since we are looking for entries similar to this one
                {
                    _moreLikeThisList.Add(entry); //adds it to the list
                }
            }
         
        }






        //clear journal
        async void clearJournal()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            // create the file and append
            StorageFile journalFile;
            string fileText = "";
            try
            {
                journalFile = await storageFolder.GetFileAsync("journal.txt");
                fileText = await Windows.Storage.FileIO.ReadTextAsync(journalFile);

            }
            catch (Exception myE)
            {
                string message = myE.Message;
                journalFile = await storageFolder.CreateFileAsync("journal.txt");
            }
            await Windows.Storage.FileIO.WriteTextAsync(journalFile,"");

        }



    }
}
