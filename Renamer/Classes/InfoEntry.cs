#region SVN Info
/***************************************************************
 * $Author$
 * $Revision$
 * $Date$
 * $LastChangedBy$
 * $LastChangedDate$
 * $URL$
 * 
 * License: GPLv3
 * 
****************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Renamer.Classes.Configuration.Keywords;
using System.Text.RegularExpressions;
using System.IO;
using System.Data;
using System.Timers;
using Renamer.Logging;
using Renamer.Dialogs;
using System.Windows.Forms;
using System.ComponentModel;
namespace Renamer.Classes
{
  /// <summary>
  /// Contains all information about a single video or subtitle file, including scheduled renaming info
  /// </summary>
  public class InfoEntry
  {

    #region Enumerations
    /// <summary>
    /// what to do with Umlauts
    /// </summary>
    public enum UmlautAction : int { Unset, Ignore, Use, Dont_Use };

    /// <summary>
    /// what to do with casing of filenames
    /// </summary>
    public enum Case : int { Unset, Ignore, small, UpperFirst, CAPSLOCK };

    /// <summary>
    /// if file should be moved
    /// </summary>
    public enum DirectoryStructure : int { Unset, CreateDirectoryStructure, NoDirectoryStructure };
    #endregion


    #region members
    private Filepath source;
    private Filepath destination;

    private string nameOfSeries;
    private string nameOfEpisode;

    private int season;
    private int episode;

    private bool isVideofile;
    private bool isSubtitle;
    private bool processingRequested;
    private bool isMovie;
    private bool isSample;
    private bool markedForDeletion;

    private UmlautAction umlautUsage;
    private Case casing;
    private DirectoryStructure createDirectoryStructure;
    private Helper.Languages language;
    #endregion

    #region Static Members
    private static string[] toBeReplaced = { "ä", "Ä", "ö", "Ö", "ü", "Ü", "ß", "É", "È", "Ê", "Ë", "Á", "À", "Â", "Ã", "Å", "Í", "Ì", "Î", "Ï", "Ú", "Ù", "Û", "Ó", "Ò", "Ô", "Ý", "Ç", "é", "è", "ê", "ë", "á", "à", "â", "ã", "å", "í", "ì", "î", "ï", "ú", "ù", "û", "ó", "ò", "ô", "ý", "ÿ", "ç" };
    private static string[] toBeReplacedWith = { "ae", "Ae", "oe", "Oe", "ue", "Ue", "ss", "E", "E", "E", "E", "A", "A", "A", "A", "A", "I", "I", "I", "I", "U", "U", "U", "O", "O", "O", "Y", "C", "e", "e", "e", "e", "a", "a", "a", "a", "a", "i", "i", "i", "i", "u", "u", "u", "o", "o", "o", "y", "y", "c" };
    public static string NotRecognized = "Not recognized, please enter manually";
    #endregion

    #region Properties

    /// <summary>
    /// destination directory
    /// </summary>
    public string Destination
    {
      get
      {
        if ( MarkedForDeletion )
        {
          return "To be deleted";
        }
        else
        {
          return destination.Path;
        }
      }
      set { destination.Path = value; }
    }
    /// <summary>
    /// new filename with extension
    /// </summary>
    public string NewFilename
    {
      get
      {
        if ( MarkedForDeletion )
        {
          return "To be deleted";
        }
        else
        {
          if ( String.IsNullOrEmpty ( Showname ) )
          {
            return "";
          }
          else
          {
            return this.destination.Filename;
          }
        }
      }
      set
      {
        this.destination.Filename = value;
      }
    }

    /// <summary>
    /// Old filename with extension
    /// </summary>
    public string Filename
    {
      get { return source.Filename; }
      set
      {
        if ( source.Filename != value )
        {
          source.Filename = value;
          if ( source.Filename != "" )
          {
            CheckExtension ();
            if ( source.Path != "" )
            {
              ExtractName ();
            }
          }
        }
      }
    }
    /// <summary>
    /// Extension of the file without dot, i.e. "avi" or "srt"
    /// </summary>
    public string Extension
    {
      get { return source.Extension; }
      set
      {
        if ( source.Extension != value )
        {
          source.Extension = value;
          destination.Extension = value;
          CheckExtension ();
          ExtractName ();
        }
      }
    }

    /// <summary>
    /// Path of the file
    /// </summary>
    public Filepath FilePath
    {
      get { return source; }
      set
      {
        source = value;
      }
    }
    /// <summary>
    /// Name of the show this file belongs to.
    /// </summary>
    public string Showname
    {
      get { return nameOfSeries; }
      set
      {
        if ( value == NotRecognized ) value = "";
        if ( nameOfSeries != value )
        {
          if ( value == null ) value = "";
          if ( value == "Sample" && Helper.ReadBool ( Config.DeleteSampleFiles ) )
          {
            MarkedForDeletion = true;
          }
          nameOfSeries = value;
          //need to find showname again incase there is different data for new showname
          if ( !Movie )
          {
            SetupRelation ();
          }
          //filename might contain showname, so it needs to be updated anyway
          CreateNewName ();
          //directory structure also contains showname and should be updated
          SetPath ();
        }
      }
    }

    /// <summary>
    /// number of the season
    /// </summary>
    public int Season
    {
      get { return season; }
      set
      {
        if ( season != value )
        {
          season = value;
          CreateNewName ();
          SetPath ();
          SetupRelation ();
        }
      }
    }
    /// <summary>
    /// number of the episode
    /// </summary>
    public int Episode
    {
      get { return episode; }
      set
      {
        if ( episode != value )
        {
          episode = value;
          SetupRelation ();
          CreateNewName ();
        }
      }
    }
    /// <summary>
    /// name of the episode
    /// </summary>
    public string Name
    {
      get { return nameOfEpisode; }
      set
      {
        if ( nameOfEpisode != value )
        {
          nameOfEpisode = value;
          SetPath ();
          CreateNewName ();
        }
      }
    }

    public bool IsVideofile
    {
      get
      {
        return this.isVideofile;
      }
    }
    public bool IsSubtitle
    {
      get
      {
        return this.isSubtitle;
      }
    }

    /// <summary>
    /// If file is a movie.
    /// </summary>
    public bool Movie
    {
      get { return isMovie; }
      set
      {
        if ( isMovie != value )
        {
          isMovie = value;
          if ( Showname != "" )
          {
            CreateNewName ();
            SetPath ();
          }
        }
      }
    }
    /// <summary>
    /// If file is to be processed
    /// </summary>
    public bool ProcessingRequested
    {
      get { return processingRequested; }
      set { processingRequested = value; }
    }

    public bool Sample
    {
      get { return isSample; }
      set { isSample = value; }
    }
    /// <summary>
    /// If file is to be deleted
    /// </summary>
    public bool MarkedForDeletion
    {
      get { return markedForDeletion; }
      set { markedForDeletion = value; }
    }

    /// <summary>
    /// from which level in the directory structure the name was extracted, 0=filename, 1=name file is in, 2= parent of 1, etc
    /// </summary>
    public int ExtractedNameLevel
    {
      get;
      set;
    }
    /// <summary>
    /// Option indicates the using of umlaute
    /// </summary>
    public UmlautAction UmlautUsage
    {
      get { return umlautUsage; }
      set
      {
        if ( umlautUsage != value )
        {
          umlautUsage = value;
          CreateNewName ();
          SetPath ();
        }
      }
    }
    /// <summary>
    /// Option indicates the use of UPPER and lowercase
    /// </summary>
    public Case Casing
    {
      get { return casing; }
      set
      {
        if ( casing != value )
        {
          casing = value;
          CreateNewName ();
          SetPath ();
        }
      }
    }

    public DirectoryStructure CreateDirectoryStructure
    {
      get { return createDirectoryStructure; }
      set
      {
        if ( createDirectoryStructure != value )
        {
          createDirectoryStructure = value;
          SetPath ();
        }
      }
    }
    public Helper.Languages Language
    {
      get { return language; }
      set
      {
        if ( language != value )
        {
          language = value;
          if ( source.Name != "" ) SetPath ();
          if ( NewFilename != "" ) CreateNewName ();
        }
      }
    }
    /// <summary>
    /// True if the file is located in a season folder, false if uninitialized or otherwise
    /// </summary>
    public bool InSeasonFolder
    {
      get
      {
        if ( string.IsNullOrEmpty ( FilePath.Path ) ) return false;
        string[] patterns = Helper.ReadProperties ( Config.Extract );
        foreach ( string pattern in patterns )
        {
          if ( Regex.IsMatch ( FilePath.Path, pattern ) )
            return true;
        }
        return false;
      }
    }
    public bool IsMultiFileMovie
    {
      get;
      set;
    }

    #endregion

    #region Private properties


    #endregion

    #region private Methods
    private void initMembers ()
    {
      source = new Filepath ();
      destination = new Filepath ();

      nameOfSeries = "";
      nameOfEpisode = "";

      season = -1;
      episode = -1;

      isVideofile = false;
      isSubtitle = false;
      processingRequested = true;
      isMovie = false;

      umlautUsage = UmlautAction.Unset;
      casing = Case.Unset;
      createDirectoryStructure = DirectoryStructure.Unset;
      language = Helper.Languages.None;
      ExtractedNameLevel = 0;
    }


    /// <summary>
    /// Extracts the showname of the filename
    /// </summary>
    private void ExtractName ()
    {
      if ( !source.isEmpty )
      {
        if ( Movie )
        {
          Showname = MovieNameExtractor.Instance.ExtractMovieName ( this );
        }
        else
        {
          Showname = SeriesNameExtractor.Instance.ExtractSeriesName ( this );
        }
        if ( Showname == "Sample" )
        {
          Sample = true;
        }
      }
    }

    /// <summary>
    /// set Properties depending on the Extension of the file
    /// </summary>
    private void CheckExtension ()
    {
      this.isVideofile = ( new List<string> ( Helper.ReadProperties ( Config.Extensions, true ) ) ).Contains ( this.source.Extension );
      this.isSubtitle = ( new List<string> ( Helper.ReadProperties ( Config.SubtitleExtensions, true ) ) ).Contains ( this.source.Extension );
    }


    private void SetMoviesPath ()
    {
      getCreateDirectory ();
      if ( createDirectoryStructure != DirectoryStructure.CreateDirectoryStructure )
      {
        Destination = "";
        return;
      }
      string DestinationPath = Helper.ReadProperty ( Config.DestinationDirectory );

      if ( !Directory.Exists ( DestinationPath ) )
      {
        DestinationPath = Filepath.goUpwards ( FilePath.Path, ExtractedNameLevel );
      }
      if ( IsMultiFileMovie )
      {
        DestinationPath = getMoviesDestinationPath ( DestinationPath );
      }
      if ( DestinationPath != FilePath.Path )
      {
        Destination = DestinationPath;
      }
      else
      {
        Destination = "";
      }

    }

    private string getMoviesDestinationPath ( string currentDestination )
    {
      string[] folders = Helper.splitFilePath ( currentDestination );
      if ( folders.Length > 0 )
      {
        if ( Helper.InitialsMatch ( folders[folders.Length - 1], MovieNameWithoutPart () ) )
        {
          return currentDestination;
        }
      }
      return Filepath.goIntoFolder ( currentDestination, MovieNameWithoutPart () );
    }

    private string MovieNameWithoutPart ()
    {
      return Showname.Substring ( 0, Showname.Length - 3 );
    }

    // TODO: function is still tooooooo large
    private void SetSeriesPath ()
    {
      if ( Season == -1 || Showname == "" )
      {
        Destination = "";
        return;
      }
      //string basepath = Helper.ReadProperty(Config.LastDirectory);
      string DestinationPath = Helper.ReadProperty ( Config.DestinationDirectory );

      if ( !Directory.Exists ( DestinationPath ) )
      {
        DestinationPath = FilePath.Path;
      }
      bool DifferentDestinationPath = FilePath.Path != DestinationPath;
      //for placing files in directory structure, figure out if selected directory is show name, otherwise create one
      bool isNetwork = FilePath.Path.StartsWith ( "" + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar );
      string[] dirs = this.source.Folders;
      bool InSeriesDir = false;
      bool InSeasonDir = false;
      //any (other) season dir
      bool InASeasonDir = false;
      bool UseSeasonSubDirs = Helper.ReadBool ( Config.UseSeasonSubDir );
      //figure out if we are in a season dir
      string[] seasondirs = Helper.ReadProperties ( Config.Extract );
      string aSeasondir = "";
      int showdirlevel = 0;

      //figure out if we are in an extraction dir, if we are, we need to go upwards one level
      if ( dirs.Length > 0 && Filepath.IsExtractionDirectory ( dirs[dirs.Length - 1] ) )
      {
        DestinationPath = Filepath.goUpwards ( DestinationPath, 1 );
        List<string> blah = new List<string> ( dirs );
        blah.RemoveAt ( dirs.Length - 1 );
        dirs = blah.ToArray ();
      }

      //check if we are in a season and/or series directory
      //loop backwards so first season entry is used if nothing is recognized and folder has to be created
      for ( int i = seasondirs.Length - 1; i >= 0; i-- )
      {
        aSeasondir = RegexConverter.replaceSeriesname ( seasondirs[i], nameOfSeries );
        bool InSomething = false;
        if ( dirs.Length > 1 )
        {
          Match m = Regex.Match ( dirs[dirs.Length - 1], aSeasondir );
          int parsedSeason;
          Int32.TryParse ( m.Groups["Season"].Value, out parsedSeason );
          if ( m.Success )
          {
            if ( parsedSeason == season )
            {
              InSeasonDir = true;
            }
            InASeasonDir = true;
            InSomething = true;
          }
        }

        //remove dots to avoid problems with series like "N.C.I.S." or "Dr. House"
        /*if (dirs.Length > 0 && dirs[dirs.Length - 1].Replace(".","").StartsWith(nameOfSeries.Replace(".",""))){
            InSeriesDir=true;
        }*/
        if ( dirs.Length > 0 && Helper.InitialsMatch ( dirs[dirs.Length - 1], nameOfSeries ) )
        {
          InSeriesDir = true;
        }
        /*else if (dirs.Length > 1 && dirs[dirs.Length - 2].Replace(".", "").StartsWith(nameOfSeries.Replace(".", "")))*/
        else if ( dirs.Length > 1 && Helper.InitialsMatch ( dirs[dirs.Length - 2], nameOfSeries ) )
        {
          InSeriesDir = true;
          showdirlevel = 1;
        }
        if ( InSomething ) break;
      }

      getCreateDirectory ();
      if ( createDirectoryStructure != DirectoryStructure.CreateDirectoryStructure || !isSeasonValid () )
      {
        Destination = "";
        return;
      }
      //if files aren't meant to be moved somewhere else
      if ( !DifferentDestinationPath )
      {
        //somewhere else, create new series dir
        if ( !InSeriesDir && !InSeasonDir && !InASeasonDir )
        {
          DestinationPath = addSeriesDir ( DestinationPath );
          DestinationPath = addSeasonsDirIfDesired ( DestinationPath );
        }
        //in series dir, create seasons dir
        else if ( InSeriesDir && !InASeasonDir )
        {
          DestinationPath = addSeasonsDirIfDesired ( DestinationPath );
        }
        //wrong season dir, add real seasons dir
        else if ( InSeriesDir && InASeasonDir && !InSeasonDir )
        {
          DestinationPath = Filepath.goUpwards ( DestinationPath, 1 );
          if ( showdirlevel == 0 )
          {
            DestinationPath = addSeriesDir ( DestinationPath );
          }
          DestinationPath = addSeasonsDirIfDesired ( DestinationPath );
        }
        //wrong show dir, go back two levels and add proper dir structure
        else if ( !InSeriesDir && InASeasonDir )
        {
          DestinationPath = addSeriesDir ( Filepath.goUpwards ( DestinationPath, 2 ) );
          DestinationPath = addSeasonsDirIfDesired ( DestinationPath );
        }
      }
      //if they should be moved
      else
      {
        DestinationPath = addSeriesDir ( DestinationPath );
        DestinationPath = addSeasonsDirIfDesired ( DestinationPath );
      }
      if ( DestinationPath != FilePath.Path )
      {
        Destination = DestinationPath;
      }
      else
      {
        Destination = "";
      }
    }
    private void getCreateDirectory ()
    {
      if ( createDirectoryStructure != DirectoryStructure.Unset )
        return;
      loadSettingCreateDirectory ();
    }
    private void loadSettingCreateDirectory ()
    {
      createDirectoryStructure = ( Helper.ReadBool ( Config.CreateDirectoryStructure ) ) ? DirectoryStructure.CreateDirectoryStructure : DirectoryStructure.NoDirectoryStructure;
    }

    private bool isSeasonValid ()
    {
      return this.season >= 0;
    }



    private string addSeriesDir ( string path )
    {
      return path + System.IO.Path.DirectorySeparatorChar + nameOfSeries;
    }

    private string addSeasonsDirIfDesired ( string path )
    {
      if ( useSeasonSubDirs () )
      {
        string[] dirs = path.Split ( new char[] { Path.DirectorySeparatorChar } );
        //figure out if we are in a season dir
        string[] seasondirs = Helper.ReadProperties ( Config.Extract );
        string aSeasondir = "";

        if ( Directory.Exists ( path ) )
        {
          string[] Directories = Directory.GetDirectories ( path );
          for ( int i = 0; i < seasondirs.Length; i++ )
          {
            foreach ( string dir in Directories )
            {
              aSeasondir = RegexConverter.replaceSeriesnameAndSeason ( seasondirs[i], nameOfSeries, season );
              if ( dirs.Length > 1 )
              {
                Match m = Regex.Match ( dir, aSeasondir );

                if ( m.Success )
                {
                  return path + System.IO.Path.DirectorySeparatorChar + Path.GetFileName ( dir );
                }
              }
            }
          }
        }
      }
      return path + ( ( useSeasonSubDirs () ) ? seasonsSubDir () : "" );
    }

    private bool useSeasonSubDirs ()
    {
      return Helper.ReadBool ( Config.UseSeasonSubDir );
    }

    private string seasonsSubDir ()
    {
      string seasondir = RegexConverter.replaceSeriesnameAndSeason ( Helper.ReadProperties ( Config.Extract )[0], nameOfSeries, season );
      return System.IO.Path.DirectorySeparatorChar + seasondir;
    }


    #endregion
    #region public Methods
    public InfoEntry ()
    {
      initMembers ();
    }

    /// <summary>
    /// Generates the file and directory name the file should be stored
    /// </summary>
    public void SetPath ()
    {
      if ( String.IsNullOrEmpty ( nameOfSeries ) )
      {
        Destination = "";
      }
      if ( !isMovie )
      {
        SetSeriesPath ();
      }
      else
      {
        SetMoviesPath ();
      }
    }

    /// <summary>
    /// This function tries to find an episode name which matches the showname, episode and season number by looking at previously downloaded relations
    /// </summary>
    public void SetupRelation ()
    {
      findEpisodeName ( RelationManager.Instance.GetRelationCollection ( this.Showname ) );
    }

    public void findEpisodeName ( RelationCollection rc )
    {
      resetName ();
      if ( rc == null )
        return;

      for ( int i = 0; i < rc.Count; i++ )
      {
        if ( isValidRelation ( rc[i] ) )
        {
          Name = rc[i].Name;
          break;
        }
        if ( isInValidSeason ( rc[i] ) || isInValidEpisode ( rc[i] ) )
          Name = rc[i].Name;
      }
    }

    private bool isValidRelation ( Relation relation )
    {
      return relation.Season == season && relation.Episode == episode;
    }

    private bool isInValidEpisode ( Relation relation )
    {
      return relation.Season == season && episode == -1;
    }

    private bool isInValidSeason ( Relation relation )
    {
      return relation.Episode == episode && season == -1;
    }

    private void resetName ()
    {
      this.Name = "";
    }

    public void adjustSpelling ( ref string input, bool extension )
    {
      input = adjustUmlauts ( input );
      input = adjustCasing ( input, extension );
    }

    private string adjustUmlauts ( string input )
    {
      UmlautAction ua = readUmlautUsage ();
      if ( ua == UmlautAction.Use && language == Helper.Languages.German )
      {
        input = transformDoubleLetterToUmalauts ( input );
      }
      else if ( ua == UmlautAction.Dont_Use )
      {
        input = replaceUmlautsAndSpecialChars ( input );
      }
      return input;
    }

    private UmlautAction readUmlautUsage ()
    {
      UmlautAction ua = umlautUsage;
      if ( ua == UmlautAction.Unset )
      {
        ua = Helper.ReadEnum<UmlautAction> ( Config.Umlaute );
        if ( ua == UmlautAction.Unset )
          ua = UmlautAction.Ignore;
      }
      return ua;
    }

    private string adjustCasing ( string input, bool extension )
    {
      if ( casing == Case.Unset )
      {
        casing = Helper.ReadEnum<Case> ( Config.Case );
        if ( casing == Case.Unset )
          casing = Case.Ignore;
      }
      if ( casing == Case.small )
      {
        input = input.ToLower ();
      }
      else if ( casing == Case.UpperFirst )
      {
        if ( !extension )
        {
          Regex r = new Regex ( @"\b(\w)(\w+)?\b", RegexOptions.Multiline | RegexOptions.IgnoreCase );
          input = Helper.UpperEveryFirst ( input );
        }
        else
        {
          input = input.ToLower ();
        }
      }
      else if ( casing == Case.CAPSLOCK )
      {
        input = input.ToUpper ();
      }
      return input;
    }

    private string replaceUmlautsAndSpecialChars ( string input )
    {
      for ( int index = 0; index < toBeReplaced.Length; index++ )
      {
        input = input.Replace ( toBeReplaced[index], toBeReplacedWith[index] );
      }
      return input;
    }
    private string transformDoubleLetterToUmalauts ( string input )
    {
      input = input.Replace ( "ae", "ä" );
      input = input.Replace ( "Ae", "Ä" );
      input = input.Replace ( "oe", "ö" );
      input = input.Replace ( "Oe", "Ö" );
      input = input.Replace ( "ue", "ü" );
      input = input.Replace ( "Ue", "Ü" );
      input = input.Replace ( "eü", "eue" );
      return input;
    }


    /// <summary>
    /// This function generates a new filename from the Target Pattern, episode, season, title, showname,... values
    /// </summary>
    public void CreateNewName ()
    {
      if ( ( Movie && Showname == "" ) || ( !Movie && !isSubtitle && nameOfEpisode == "" ) )
      {
        NewFilename = "";
        return;
      }
      else
      {
        //Note: Subtitle destination path is set here too
        if ( isSubtitle )
        {
          if ( nameOfEpisode == "" && Season > -1 && Episode > -1 )
          {
            InfoEntry videoEntry = InfoEntryManager.Instance.GetMatchingVideo ( Showname, Season, Episode );
            if ( videoEntry != null )
            {
              string nfn, dst;
              if ( videoEntry.NewFilename == "" )
              {
                nfn = Path.GetFileNameWithoutExtension ( videoEntry.Filename );
              }
              else
              {
                nfn = Path.GetFileNameWithoutExtension ( videoEntry.NewFilename );
              }
              nfn += "." + Extension;

              //Move to Video file
              dst = videoEntry.Destination;

              //Don't do this if name fits already
              if ( nfn == Filename )
              {
                nfn = "";
              }
              //Don't do this if path fits already
              if ( dst == FilePath.Path )
              {
                dst = "";
              }
              NewFilename = nfn;
              Destination = dst;
              return;
            }
            else
            {
              return;
            }
          }
          else
          {
            return;
          }
        }
        //Target Filename format
        string tmpname;

        //Those 3 strings need case/Umlaut processing
        string epname = this.nameOfEpisode;
        string seriesname = nameOfSeries;
        string extension = Extension;

        if ( !isMovie )
        {
          tmpname = Helper.ReadProperty ( Config.TargetPattern );
          tmpname = tmpname.Replace ( "%e", episode.ToString () );
          tmpname = tmpname.Replace ( "%E", episode.ToString ( "00" ) );
          tmpname = tmpname.Replace ( "%s", season.ToString () );
          tmpname = tmpname.Replace ( "%S", season.ToString ( "00" ) );
          adjustSpelling ( ref epname, false );
        }
        else
        {
          tmpname = "%T";
        }


        adjustSpelling ( ref seriesname, false );
        adjustSpelling ( ref extension, true );

        //Now that series title, episode title and extension are properly processed, add them to the filename

        //Remove extension from target filename (if existant) and add properly cased one
        tmpname = Regex.Replace ( tmpname, "\\." + extension, "", RegexOptions.IgnoreCase );
        tmpname += "." + extension;

        tmpname = tmpname.Replace ( "%T", seriesname );
        tmpname = tmpname.Replace ( "%N", epname );

        //string replace function
        List<string> replace = new List<string> ( Helper.ReadProperties ( Config.Replace ) );
        List<string> from = new List<string> ();
        List<string> to = new List<string> ();
        foreach ( string s in replace )
        {
          if ( !s.StartsWith ( Settings.Instance.Comment ) )
          {
            string[] replacement = s.Split ( new string[] { "->" }, StringSplitOptions.RemoveEmptyEntries );
            if ( replacement != null && replacement.Length == 2 )
            {
              tmpname = Regex.Replace ( tmpname, replacement[0], replacement[1], RegexOptions.IgnoreCase );
            }
          }
        }

        //set new filename if renaming process is required
        if ( Filename == tmpname )
        {
          NewFilename = "";
        }
        else
        {
          NewFilename = tmpname;
        }
      }
    }

    public void RemoveVideoTags ()
    {
      this.ProcessingRequested = false;
      //Go through all selected files and remove tags and clean them up
      this.Destination = "";
      Episode = -1;
      Season = -1;
      this.Movie = true;
      this.Showname = MovieNameExtractor.Instance.ExtractMovieName ( this );

      if ( this.NewFilename != "" || this.Destination != "" )
      {
        this.ProcessingRequested = true;
      }
    }
    public void MarkAsTVShow ()
    {
      this.ProcessingRequested = false;
      //Go through all selected files and remove tags and clean them up
      this.Destination = "";
      this.Movie = false;
      this.Showname = SeriesNameExtractor.Instance.ExtractSeriesName ( this );

      if ( this.NewFilename != "" || this.Destination != "" )
      {
        this.ProcessingRequested = true;
      }
    }
    public string GetFormattedFullDestination ()
    {
      string dest = "";
      if ( Destination != "" )
      {
        dest = Destination;
      }
      else
      {
        dest = FilePath.Path;
      }
      if ( NewFilename != "" )
      {
        dest += Path.DirectorySeparatorChar + NewFilename;
      }
      else
      {
        dest += Path.DirectorySeparatorChar + Filename;
      }
      return dest;
    }
    public void Rename ( BackgroundWorker worker, DoWorkEventArgs e )
    {
      if ( this.ProcessingRequested
          && ( ( this.Filename != this.NewFilename && this.NewFilename != "" )
              || ( this.Destination != this.FilePath.Path && this.Destination != "" ) ) )
      {
        try
        {
          //create directory if needed
          if ( this.Destination != "" && !Directory.Exists ( this.Destination ))
          {
            // verify the path has only invalid chars within it
            if ( Helper.FilePathIsValid ( this.Destination ) )
            {
              // handle invalid chars
              foreach ( char c in Helper.InvalidPathChars )
              {
                this.Destination = this.Destination.Replace ( c.ToString (), "" );
              }
            }
            Directory.CreateDirectory ( this.Destination );
          }

          //Move to desired destination      
          string src = this.FilePath.Path + Path.DirectorySeparatorChar + this.Filename;
          string target = "";
          if ( this.Destination != "" )
          {
            if ( this.NewFilename != "" )
            {
              target = this.Destination + Path.DirectorySeparatorChar + this.NewFilename;
            }
            else
            {
              target = this.Destination + Path.DirectorySeparatorChar + this.Filename;
            }
          }
          else
          {
            if ( this.NewFilename != "" )
            {
              target = this.FilePath.Path + Path.DirectorySeparatorChar + this.NewFilename;
            }
          }

          if ( target != "" )
          {
            if ( target.ToLower ()[0] == src.ToLower ()[0] )
            {
              File.Move ( src, target );
            }
            else
            {
              FileRoutines.CopyFile ( new FileInfo ( src ), new FileInfo ( target ), CopyFileOptions.AllowDecryptedDestination | CopyFileOptions.FailIfDestinationExists, new CopyFileCallback ( InfoEntryManager.Instance.ReportSingleFileProgress ) );
              if ( worker.CancellationPending )
              {
                return;
              }
              File.Delete ( src );
            }
          }

          //Refresh values
          if ( this.NewFilename != "" )
          {
            this.Filename = this.NewFilename;
          }
          if ( this.Destination != "" )
          {
            this.FilePath.Path = this.Destination;
          }
          this.Destination = "";
          this.NewFilename = "";
          //This will always be false, but might be needed elsewhere sometime
          ShouldBeProcessed ();
        }
        catch ( Exception ex )
        {
          //if the user didn't want to cancel, this is an actually error message and needs to be displayed
          if ( !worker.CancellationPending )
          {
            Logger.Instance.LogMessage ( "Couldn't move " + this.FilePath.Path + Path.DirectorySeparatorChar + this.Filename + " -> " + this.Destination + Path.DirectorySeparatorChar + this.NewFilename + ": " + ex.Message, LogLevel.ERROR );
          }
        }
      }
    }
    public void ShouldBeProcessed ()
    {
      ProcessingRequested = Destination != "" || NewFilename != "";
    }
    public string ToString ()
    {
      return Filename;
    }
    #endregion
  }
}
