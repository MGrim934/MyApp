using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources.Core;
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
    /// 
    

        //Mark Grimes

    public sealed partial class MainPage : Page
    {

        List <Mood> _moodList; 
        //takes in moods from json
        List<Activity> _activityList;
        //takes in activities
        ArrayList _previousEntries = new ArrayList();
        //the journal
        Random roll = new Random();
        //this is used when populating the pick an entry grid, to keep it random every time the user runs the program
        ArrayList moodDupCheck = new ArrayList(); //ensures that there are no duplicate entries
        ArrayList actDupCheck = new ArrayList(); // ditto
        ArrayList _moreLikeThisList = new ArrayList(); //when the user wants a more specific list
        Boolean bookOpen = false; //background image check

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
            //fills up the previous entries with the file associated with it
            lvJournal.ItemsSource = _previousEntries;
            //sets the list as previous entries




            // lvMoods.ItemsSource = _moodList;


        }



        private async void loadLocalData()
        {
            var moodsFile = await Package.Current.InstalledLocation.GetFileAsync("Data\\moods.txt"); 
            //file that stores moods name + image
            var fileText = await FileIO.ReadTextAsync(moodsFile);


            try
            {
                var moodsJArray = JsonArray.Parse(fileText); 
                //parses the file
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
            //creates grid of moods
            generateActivityGrid(); 
            //creates grid of activities
            ReadJournalFile(); 
            //tries to read previous entries


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
            foreach (var item in actJArray) //for every item in the array
            {
                // get the object
                var obj = item.GetObject();


                Activity activity = new Activity();

                // get each key value pair and sort it to the appropriate elements
                // of the class
                foreach (var key in obj.Keys) //get the objects properties
                {
                    IJsonValue value;
                    if (!obj.TryGetValue(key, out value)) //if there are no more values to get, continue and break out of this meethod
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
                _activityList.Add(activity); //adds to the activity list
            } // end foreach (var item in array)
        }//createList of Moods


        private  void generateMoodGrid()
        {
            Image img;
            int moodChoice;
            //fill up grid
            for(int i = 0; i < 5; i++) //4 rows
            {
                for(int j = 0; j < 2; j++) //2 columns 
                {
                    img = new Image();

                    do
                    {
                       moodChoice = roll.Next(0, (_moodList.Count)); //add something between 0 and the length of the mood list
                    } while (moodDupCheck.Contains(moodChoice)); //if its already been added, don't add it


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
                    img.Tapped += Mood_Tapped; //gives it an event handler
                    gdMood.Children.Add(img); //adds the image to the mood grid

                }
            }
        }

        private void Mood_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image curr = (Image)sender;
            //change text
            //show other grid
            gdMood.Visibility = Visibility.Collapsed; //sets the grid to not visible

            gdActivity.Visibility = Visibility.Visible; //raises the activity grid
            //get mood type
            //get activity type
            //get date
            //write to file
            JournalEntry entry = new JournalEntry(); //another class that stores journal entries
            mood = curr.Name; //curr.name "happy"

            //change text block
            ResourceCandidate r1 = ResourceManager.Current.MainResourceMap.GetValue("Resources/entryPageAlt", ResourceContext.GetForCurrentView());
            //pulls from resource file
          
            tbMood.Text = r1.ValueAsString;





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
                    } while (actDupCheck.Contains(activityChoice)); //prevents duplication


                    img.Name = _activityList[activityChoice].name;
                   actDupCheck.Add(activityChoice); //adds it to the dup list to prevent dups
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
            //change text block
            ResourceCandidate r1 = ResourceManager.Current.MainResourceMap.GetValue("Resources/uidEntryPage/Text", ResourceContext.GetForCurrentView());

            tbMood.Text = r1.ValueAsString;



            createJournalEntry(); //triggers a new instance of a journal entry
        }



        private void createJournalEntry()
        {
            JournalEntry entry = new JournalEntry();
            int index = _moodList.FindIndex(item => item.name.Equals(mood));  
            //finds if any item in the mood list, has a property that equals the property specified
           
            entry.mood =_moodList[index].image; 
            //if it does, it sets the image path as the image path defined in the list

            index = _activityList.FindIndex(item => item.name.Equals(activity));

            entry.activity = _activityList[index].image;
            
          
            entry.dateTime = DateTime.Now;
            //add to list
            _previousEntries.Insert(0,entry);
            //adds it to the top


            JournalEntry(entry);
            //adds journal to entry file for storage


            updateJournal();
            ptMain.SelectedIndex = 2; 
            //moves to previous entries page
           


        }

        private void tbNewEntry_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //change selected index
            ptMain.SelectedIndex = 1; //moves to a new entry page
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

            

            gdMaster.Background = fill; //opens the book 
            bookOpen = true;
        }

        private void closeBook(object sender, TappedRoutedEventArgs e)
        {
            Pivot curr = (Pivot)sender;

            if (curr.SelectedIndex == 0) { 
            Uri pic = new Uri("ms-appx:///Images/main1.png", UriKind.RelativeOrAbsolute);
            ImageBrush fill = new ImageBrush();
            fill.ImageSource = new BitmapImage(pic); //closes the book
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
            

        }

        async void ReadJournalFile()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile journalFile;

            try
            {
                journalFile = await storageFolder.GetFileAsync("journal.txt"); //tries to get this file
            }
            catch (Exception myE)
            {
                string message = myE.Message;
                return;
            }

            string fileText=await Windows.Storage.FileIO.ReadTextAsync(journalFile); //reads from file

            //run method that parses this string and adds elements to the array from it
           // tbTest.Text = fileText;
            AddToJournal(fileText);

        }

         void AddToJournal(String text)
        {
            char[] charSeperators = new char[] { ' ', ',' ,'\n'};
            string[] words = text.Split(charSeperators,StringSplitOptions.RemoveEmptyEntries);
            //splits up based on ,

            for(int i = 0; i < words.Length; i+=4) {
                //create a new entry
                if ((i + 4) < words.Length)
                {
                    JournalEntry entry = new JournalEntry();
                    entry.mood = words[i];
                    entry.activity = words[i + 1];


                    string date = words[i + 2]+" "+words[i+3];
                    entry.dateTime = Convert.ToDateTime(date);
                    _previousEntries.Add(entry);
                }
                
            }
            _previousEntries.Reverse(); //does this so it displays the most recent at the top


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
            jLine = entry.mood + "," + entry.activity + "," + entry.dateTime; //, lets me use split method easily
            await Windows.Storage.FileIO.WriteTextAsync(journalFile, fileText + jLine + System.Environment.NewLine);
            //test
           // tbTest.Text= tbTest.Text + jLine + System.Environment.NewLine;

        }

        //if the user clicks to clear the journal
        private void btnClearQ_Tapped(object sender, TappedRoutedEventArgs e)
        {

            ResourceCandidate r1 = ResourceManager.Current.MainResourceMap.GetValue("Resources/BtClearConfirm", ResourceContext.GetForCurrentView());
          
            btnClearQ.Content = r1.ValueAsString; //gotta make sure
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
            ResourceCandidate r1 = ResourceManager.Current.MainResourceMap.GetValue("Resources/uidBtClearQ/Content", ResourceContext.GetForCurrentView());
            btnClearQ.Content = r1.ValueAsString;
            btnClearQ.Tapped += btnClearQ_Tapped; //lets them tap again

            btnClearY.Visibility = Visibility.Collapsed;
            btnClearN.Visibility = Visibility.Collapsed;
        }

        private void clearJournalArray()
        {
            _previousEntries.Clear(); //clears the journal array
            updateJournal(); //clears the list view
            
        }
        //==========================================================

        private void lvJournal_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //get item at selected index
           PivotlimitCheck(); //stops user from adding multiple extra pages




            ListView current = (ListView) sender;
            //find current entry
            if (_previousEntries.Count<1)
            {
                return; //breaks out of this method if there are no entries
            }
            JournalEntry entry =(JournalEntry) _previousEntries[lvJournal.SelectedIndex]; //the user selected entry
            PivotItem piNew = new PivotItem(); //generates a new pivot item
            piNew.Name = "piStat";

            ResourceCandidate r1 = ResourceManager.Current.MainResourceMap.GetValue("Resources/piStats/Header", ResourceContext.GetForCurrentView());
            piNew.Header = r1.ValueAsString;
            //need to change this
            StackPanel stk = new StackPanel();
            TextBlock block = new TextBlock();
            TextBlock header = new TextBlock();
            stk.Orientation = Orientation.Horizontal;
            
            //add images and text
            Image mood = new Image();

            mood.Width =125;
            mood.Height =125;
            
            mood.Source = new BitmapImage (new Uri("ms-appx://"+entry.mood, UriKind.RelativeOrAbsolute));
            mood.Name = entry.mood;
            mood.Tapped += more_Tapped;
            mood.Margin = new Thickness(0, 0, 10, 0);
            Image activity = new Image();
            activity.Width =150;
            activity.Height =125;
            activity.Name = entry.activity;
            activity.Tapped += more_Tapped;
            activity.Margin = new Thickness(10, 0, 0, 0);
            activity.Source = new BitmapImage(new Uri("ms-appx://" + entry.activity, UriKind.RelativeOrAbsolute));

            TextBlock date = new TextBlock();
            date.TextAlignment = TextAlignment.Center;
            date.Text = entry.dateTime.ToString();
            date.Margin = new Thickness(0, 0, 0, 20);
            date.FontSize = 36;
            stk.Children.Add(mood);
            //puts two images in this stack panel
            stk.Children.Add(activity);

            StackPanel outerStk = new StackPanel();
            outerStk.Children.Add(date); //puts the date in the stack panel above the two images
            outerStk.Children.Add(stk);
            //stk.Children.Add(date);

             r1 = ResourceManager.Current.MainResourceMap.GetValue("Resources/moreInfo", ResourceContext.GetForCurrentView());
          

            header.Text = r1.ValueAsString;
            header.Style = (Style) Resources["HeaderTextBlockStyle"];
            header.Margin = new Thickness(0, 0, 0, 40);
            header.TextAlignment = TextAlignment.Center;

            TextBlock question = new TextBlock();
            r1 = ResourceManager.Current.MainResourceMap.GetValue("Resources/moodOrAct", ResourceContext.GetForCurrentView());
            question.Text = r1.ValueAsString;


            //  question.Text = "Mood\tActivity";
            question.Style = (Style)Resources["HeaderTextBlockStyle"];
            question.FontSize = 38;
            question.Margin = new Thickness(0, 20, 0, 0);
            question.TextAlignment = TextAlignment.Center;

            StackPanel stack = new StackPanel();
            stack.Children.Add(header);

            stack.HorizontalAlignment=HorizontalAlignment.Center;
            stack.Children.Add(outerStk);
            stack.Children.Add(question);
            
           
        
         
      
         
           
            piNew.Content = stack;
            
            ptMain.Items.Add(piNew);
          
            ptMain.SelectedIndex=(ptMain.Items.Count-1);
          

            //offer user choice
            //more like this
            //mood
            //activity


        }

        private void PivotlimitCheck()
        {

            if (ptMain.Items.Count > 3)
                ptMain.Items.RemoveAt(3); //stops the user from creating too many pages

        }

        private void updateList(ListView lvMoreLikeThis)
        {
            lvMoreLikeThis.ItemsSource = null;
            lvMoreLikeThis.ItemsSource = _moreLikeThisList;
        }

        //==================================================== 

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
            ResourceCandidate r1 = ResourceManager.Current.MainResourceMap.GetValue("Resources/piStats/Header", ResourceContext.GetForCurrentView());
            piNew.Header = r1.ValueAsString; //header
            StackPanel stk = new StackPanel();
            stk.Orientation = Orientation.Horizontal;

            StackPanel stack = new StackPanel();
            TextBlock sim = new TextBlock();

            r1 = ResourceManager.Current.MainResourceMap.GetValue("Resources/similarEntries", ResourceContext.GetForCurrentView());
            sim.Text = r1.ValueAsString;
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
            ptMain.SelectedIndex=(ptMain.Items.Count-1); //moves to the new page that was generated




        }


        private void fillList(Image img)
        {
            
         
            //check against _previous entries
            //fill new arraylist
            //create listview
            //send to list view
            JournalEntry entry; //creates a new journal entry for the list
            _moreLikeThisList.Clear();
            if (isMood(img))
            {
                for (int i = 0; i < _previousEntries.Count; i++) //checks every element of the array
                {
                    entry = (JournalEntry)_previousEntries[i];
                    if (entry.mood.Equals(img.Name)) //if we've seen another instance of this mood, add it to the list. Since we are looking for entries similar to this one
                    {
                        _moreLikeThisList.Add(entry); //adds it to the list
                    }
                }

            }
            else
            {
                for (int i = 0; i < _previousEntries.Count; i++) //checks every element of the array
                {
                    entry = (JournalEntry)_previousEntries[i];
                    if (entry.activity.Equals(img.Name)) //if we've seen another instance of this activity, add it to the list. Since we are looking for entries similar to this one
                    {
                        _moreLikeThisList.Add(entry); //adds it to the list
                    }
                }
            }

        }

       Boolean isMood(Image img)
        {
            Boolean answer = false;
            //if its a mood, true
            //if its an activity, false
           

            //check against name
            int index = _moodList.FindIndex(item => item.image.Equals(img.Name)); //if there is an image with the same url
            //is it found?
            if (index > 0)
            {
                answer = true; //if its greater than 0, then it is there. If its not, then its -1 so its false
            }
            return answer;//return the answer
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
            await Windows.Storage.FileIO.WriteTextAsync(journalFile,","); 
            //overwrites file with an empty string
            //effectively clears the journal

        }



    }
}
