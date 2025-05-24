using System.ComponentModel.DataAnnotations;
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
using Microsoft.VisualBasic.FileIO;

namespace PangyaRandomizerDesign;
public partial class MainWindow : Window
{
    public int RandomizeCount = 0;
    public List<CardStats> cardstatcollection = new List<CardStats>();
    public List<CardStats> selectedcard = new List<CardStats>();
    public MainWindow()
    {
        Initialize1();
    }
    public void Initialize1()
    {
        addcardstats();
        //ResetB();
    }

    public void Calculateforrandomizing()
    {
        if (selectedcard.Count <= 0)
        {
            MessageBox.Show("ใส่การ์ด !");
            selectedcard.Clear();
            return;
        }
        if (selectedcard.Count >= 4)
        {
            MessageBox.Show("ใส่การ์ดเกิน 3 ใบ ใส่ใหม่ !");
            selectedcard.Clear();
            return;
        }
        /////////////////////////////////////////////////////////////////
        //If success => Try to assignstats before randomizing.
        RandomizeOptionStats newoption = new RandomizeOptionStats();

        //Add bonus if try to use different card.
        List<CardStats> listofuniqcard = selectedcard.Distinct().ToList();
        listofuniqcard.RemoveAll(card => card.cardname == "SC-Ore" || card.cardname == "UL-Ore");

        if (listofuniqcard.Count >= 2)
        {
            newoption.overallcardweight += listofuniqcard.Count - 1;
        }

        //Add Bonus if try to enhance the Transformed clubset.
        if (Special_Clubset_Type1.IsChecked == true)
        {
            newoption.overallcardweight += 3;
        }
        else if (Special_Clubset_Type2.IsChecked == true)
        {
            newoption.overallcardweight += 14;
            newoption.othercardweight = 24;
            newoption.buffpangnpc = 12;
            newoption.buffexpnpc = 12;
        }
        else if (Special_Clubset_Type3.IsChecked == true)
        {
            newoption.overallcardweight += 14;
            newoption.charcardweight = 14;
            newoption.impact = 14;
        }

        foreach (CardStats mycard in selectedcard)
        {
            newoption.overallcardweight += mycard.overallcardweight;
            if (mycard.typecard == 1)
            {
                //Character card
                newoption.charcardweight += mycard.charcardweight;
                newoption.power += mycard.powerslot;
                newoption.control += mycard.controlslot;
                newoption.impact += mycard.impactslot;
                newoption.spin += mycard.spinslot;
                newoption.curve += mycard.curveslot;
            }
            else if (mycard.typecard == 2)
            {
                //Caddie card
                newoption.othercardweight += mycard.othercardweight;
                newoption.speciallength += mycard.speciallength;
                newoption.speciallengthgauge += (int)(3 * (float)(mycard.speciallengthgauge / 8.0f));
                newoption.pangyapx += (int)(3 * (float)(mycard.pangyapx / 2.0f));
                newoption.initialguage += (int)(3 * (float)(mycard.initialguage / 33.0f));
                newoption.passivegauge += (int)(3 * (float)(mycard.passivegauge / 4.0f));
                newoption.windreduction += 3 * mycard.windreduction;
                newoption.pangyapxnpc += (int)(3 * (float)(mycard.pangyapxnpc / 4.0f));
                newoption.buffpangnpc += (int)(3 * (float)(mycard.buffpangnpc / 20.0f));
                newoption.buffexpnpc += (int)(3 * (float)(mycard.buffexpnpc / 20.0f));
                newoption.buffcontrolnpc += (int)(3 * (float)(mycard.buffcontrolnpc / 4.0f));
                newoption.buffdroprate += (int)(3 * (float)(mycard.buffdroprate / 4.0f));
            }
            else if (mycard.typecard == 3)
            {
                //Card Ore
                newoption.charcardweight += mycard.charcardweight;
                newoption.power += mycard.powerslot;
                newoption.control += mycard.controlslot;
                newoption.impact += mycard.impactslot;
                newoption.spin += mycard.spinslot;
                newoption.curve += mycard.curveslot;
                newoption.othercardweight += mycard.othercardweight;
                newoption.speciallength += mycard.speciallength;
                newoption.speciallengthgauge += (int)(3 * (float)(mycard.speciallengthgauge / 8.0f));
                newoption.pangyapx += (int)(3 * (float)(mycard.pangyapx / 2.0f));
                newoption.initialguage += (int)(3 * (float)(mycard.initialguage / 33.0f));
                newoption.passivegauge += (int)(3 * (float)(mycard.passivegauge / 4.0f));
                newoption.windreduction += 3 * mycard.windreduction;
                newoption.pangyapxnpc += (int)(3 * (float)(mycard.pangyapxnpc / 4.0f));
                newoption.buffpangnpc += (int)(3 * (float)(mycard.buffpangnpc / 20.0f));
                newoption.buffexpnpc += (int)(3 * (float)(mycard.buffexpnpc / 20.0f));
                newoption.buffcontrolnpc += (int)(3 * (float)(mycard.buffcontrolnpc / 4.0f));
                newoption.buffdroprate += (int)(3 * (float)(mycard.buffdroprate / 4.0f));
            }
        }
        /////////////////////////////////////////////////////////////////
        //After assign randomizer stats => try to calculate before randomize options.
        if (newoption == null)
        {
            MessageBox.Show("ตัวสุ่มทำงานผิดพลาด");
            return;
        }
        if (newoption.speciallength <= 0)
        {
            newoption.speciallength = 0;
        }
        int cardpointremaining = newoption.overallcardweight;
        int allweight = newoption.charcardweight + newoption.othercardweight;
        //int allstatsweight = newoption.power + newoption.control + newoption.impact +
        //                        newoption.spin + newoption.curve + newoption.speciallength +
        //                        newoption.speciallengthgauge + newoption.pangyapx +
        //                        newoption.initialguage + newoption.passivegauge + newoption.windreduction +
        //                        newoption.pangyapxnpc + newoption.buffpangnpc + newoption.buffexpnpc +
        //                        newoption.buffcontrolnpc + newoption.buffdroprate;
        int charcardweight = newoption.power + newoption.control + newoption.impact +
                             newoption.spin + newoption.curve;
        int othercardweight = newoption.speciallength + newoption.speciallengthgauge + newoption.pangyapx +
                                newoption.initialguage + newoption.passivegauge + newoption.windreduction +
                                newoption.pangyapxnpc + newoption.buffpangnpc + newoption.buffexpnpc +
                                newoption.buffcontrolnpc + newoption.buffdroprate;
        int charactercardscore = 0;
        int othercardscore = 0;
        // Assign to stats till card point is zero.
        while (cardpointremaining >= 0)
        {
            cardpointremaining--;

            int randomizescore = 0;
            
            //Random seed.
            Random rnd = new Random();
            int mynumber = rnd.Next(0, 6);
            //int[] digitsArray = GetIntArray(randomNumber);
            //Determine score depend on your luck
            if (mynumber == 5)
            {
                randomizescore += 3;
            }
            else if (mynumber >= 3)
            {
                randomizescore += 2;
            }
            else
            {
                randomizescore += 1;
            }

            //Now add score depend on weight on that card.
            Random rnd2 = new Random();
            int selection1 = WeightedRandomSelection(new int[]
                          { newoption.charcardweight, newoption.othercardweight }, allweight, rnd2);
            if (selection1 == 0)
            {
                charactercardscore += randomizescore;
            }
            else
            {
                othercardscore += randomizescore;
            }
        }
        //If card point exceed zero, we assign stats randomly on new object.
        RandomizedStatScore summarizedstats = new RandomizedStatScore();
        //Updatecharstats
        Random rnd3 = new Random();
        for (int i = 0; i < charactercardscore; i++)
        {
            int selection2 = WeightedRandomSelection(new int[]
                      { newoption.power, newoption.control, newoption.impact,
                        newoption.spin, newoption.curve}, charcardweight, rnd3);
            UpdateCharStat(summarizedstats, selection2);
        }
        //UpdateOtherstats
        for (int i = 0; i < othercardscore; i++)
        {
            Random rnd4 = new Random();
            int selection3 = WeightedRandomSelection(new int[]
                          { newoption.speciallength, newoption.speciallengthgauge, newoption.pangyapx,
                                newoption.initialguage, newoption.passivegauge, newoption.windreduction,
                                newoption.pangyapxnpc, newoption.buffpangnpc, newoption.buffexpnpc,
                                newoption.buffcontrolnpc, newoption.buffdroprate }, othercardweight, rnd4);
            UpdateOtherStat(summarizedstats, selection3);
        }

        // Get all stats in a single array before we do something with stats
        int[] allStats = {
            summarizedstats.power, summarizedstats.control, summarizedstats.impact, summarizedstats.spin, summarizedstats.curve, summarizedstats.speciallength,
            summarizedstats.speciallengthgauge, summarizedstats.pangyapx, summarizedstats.initialguage, summarizedstats.passivegauge,
            summarizedstats.windreduction, summarizedstats.pangyapxnpc, summarizedstats.buffpangnpc, summarizedstats.buffexpnpc,
            summarizedstats.buffcontrolnpc, summarizedstats.buffdroprate
        };

        //Debug stats Before weighted distributed randomization
        PreSumOfPoints.Content = (summarizedstats.power + summarizedstats.control + summarizedstats.impact +
                                summarizedstats.spin +summarizedstats.curve + summarizedstats.speciallength +
                                summarizedstats.speciallengthgauge + summarizedstats.pangyapx + summarizedstats.initialguage +
                                summarizedstats.passivegauge + summarizedstats.windreduction + summarizedstats.pangyapxnpc +
                                summarizedstats.buffpangnpc + summarizedstats.buffexpnpc + summarizedstats.buffcontrolnpc +
                                summarizedstats.buffdroprate).ToString();
        DistributionPointA.Content = "[" + string.Join(", ", allStats) + "]";
        //MessageBox.Show("Before : " + summarizedstats.power + ", " + summarizedstats.control + ", " + summarizedstats.impact + ", " + summarizedstats.spin + ", " + summarizedstats.curve + ", " + summarizedstats.speciallength + ", " +
        //    summarizedstats.speciallengthgauge + ", " + summarizedstats.pangyapx + ", " + summarizedstats.initialguage + ", " + summarizedstats.passivegauge + ", " +
        //    summarizedstats.windreduction + ", " + summarizedstats.pangyapxnpc + ", " + summarizedstats.buffpangnpc + ", " + summarizedstats.buffexpnpc + ", " +
        //    summarizedstats.buffcontrolnpc + ", " + summarizedstats.buffdroprate);

        // Get top 3 indices across all stats
        int accumulatedPoints = ClampStatsTo24AndAccumulateExcess(allStats, 0);
        int[] top3Indices = GetTop3Indices(allStats);
        accumulatedPoints = AccumulateAndResetStats(allStats, top3Indices, accumulatedPoints);
        // Check if we didn't already have top3 then put stats on other stats in top3
        if (accumulatedPoints > 0)
        {
            Random rnd5 = new Random();
            DistributePointsToTop3(allStats, top3Indices, accumulatedPoints, rnd5);
            
            // Apply the updated stats back
            summarizedstats.power = allStats[0];
            summarizedstats.control = allStats[1];
            summarizedstats.impact = allStats[2];
            summarizedstats.spin = allStats[3];
            summarizedstats.curve = allStats[4];
            summarizedstats.speciallength = allStats[5];
            summarizedstats.speciallengthgauge = allStats[6];
            summarizedstats.pangyapx = allStats[7];
            summarizedstats.initialguage = allStats[8];
            summarizedstats.passivegauge = allStats[9];
            summarizedstats.windreduction = allStats[10];
            summarizedstats.pangyapxnpc = allStats[11];
            summarizedstats.buffpangnpc = allStats[12];
            summarizedstats.buffexpnpc = allStats[13];
            summarizedstats.buffcontrolnpc = allStats[14];
            summarizedstats.buffdroprate = allStats[15];
        }

        //Debug stats after weighted distributed randomization
        //MessageBox.Show("After : " + summarizedstats.power + ", " + summarizedstats.control + ", " + summarizedstats.impact + ", " + summarizedstats.spin + ", " + summarizedstats.curve + ", " + summarizedstats.speciallength + ", " +
        //summarizedstats.speciallengthgauge + ", " + summarizedstats.pangyapx + ", " + summarizedstats.initialguage + ", " + summarizedstats.passivegauge + ", " +
        //summarizedstats.windreduction + ", " + summarizedstats.pangyapxnpc + ", " + summarizedstats.buffpangnpc + ", " + summarizedstats.buffexpnpc + ", " +
        //summarizedstats.buffcontrolnpc + ", " + summarizedstats.buffdroprate);

        TranslateStatsToText(summarizedstats);

        //Put option to UI
        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    Option1.Content = summarizedstats.stat[i];
                    break;
                case 1:
                    Option2.Content = summarizedstats.stat[i];
                    break;
                case 2:
                    Option3.Content = summarizedstats.stat[i];
                    break;
            }
        }
        DistributionPointB.Content = "[" + string.Join(", ", allStats) + "]";
        PostSumOfPoints.Content = (summarizedstats.power + summarizedstats.control + summarizedstats.impact +
                                summarizedstats.spin + summarizedstats.curve + summarizedstats.speciallength +
                                summarizedstats.speciallengthgauge + summarizedstats.pangyapx + summarizedstats.initialguage +
                                summarizedstats.passivegauge + summarizedstats.windreduction + summarizedstats.pangyapxnpc +
                                summarizedstats.buffpangnpc + summarizedstats.buffexpnpc + summarizedstats.buffcontrolnpc +
                                summarizedstats.buffdroprate).ToString();
        EnableButton();
        RandomizeCount++;
        RandCounterText.Content = RandomizeCount.ToString();
        //MessageBox.Show("สุ่มสถานะเรียบร้อยแล้ว");
    }
    #region Classes
    public class CardStats(string? cardname1, int typecard1, int overallcardweight1,
                           int charcardweight1, int caddiecardweight1, int powerslot1,
                           int controlslot1, int impactslot1, int spinslot1, int curveslot1,
                           int speciallength1, int speciallengthguage1, int pangyapx1,
                           int initialguage1, int passivegauge1, int windreduction1, int pangyapxnpc,
                           int buffpangnpc1, int buffexpnpc1, int buffcontrolnpc1, int buffdroprate1)
    {
        //card weight ranged from 0-5
        public string? cardname = cardname1;
        public int typecard = typecard1;
        public int overallcardweight = overallcardweight1;
        public int charcardweight = charcardweight1;
        public int othercardweight = caddiecardweight1;
        public int powerslot = powerslot1;
        public int controlslot = controlslot1;
        public int impactslot = impactslot1;
        public int spinslot = spinslot1;
        public int curveslot = curveslot1;
        public int speciallength = speciallength1;
        public int speciallengthgauge = speciallengthguage1;
        public int pangyapx = pangyapx1;
        public int initialguage = initialguage1;
        public int passivegauge = passivegauge1;
        public int windreduction = windreduction1;
        public int pangyapxnpc = pangyapxnpc;
        public int buffpangnpc = buffpangnpc1;
        public int buffexpnpc = buffexpnpc1;
        public int buffcontrolnpc = buffcontrolnpc1;
        public int buffdroprate = buffdroprate1;
    }
    public class RandomizeOptionStats
    {
        //range from 0-5
        public int overallcardweight;
        public int charcardweight;
        public int othercardweight;
        public int power;
        public int control;
        public int impact;
        public int spin;
        public int curve;
        public int speciallength;
        public int speciallengthgauge;
        public int pangyapx;
        public int initialguage;
        public int passivegauge;
        public int windreduction;
        public int pangyapxnpc;
        public int buffpangnpc;
        public int buffexpnpc;
        public int buffcontrolnpc;
        public int buffdroprate;
    }
    public class RandomizedStatScore
    {
        //range from 0-5
        public int power;
        public int control;
        public int impact;
        public int spin;
        public int curve;
        public int speciallength;
        public int speciallengthgauge;
        public int pangyapx;
        public int initialguage;
        public int passivegauge;
        public int windreduction;
        public int pangyapxnpc;
        public int buffpangnpc;
        public int buffexpnpc;
        public int buffcontrolnpc;
        public int buffdroprate;
        public List<string> stat = new List<string>();
    }
    #endregion
    #region CardStats
    public void addcardstats()
    {
        //Slot 2 = Type => 1 = Character, 2 = Caddie, 3 = normal;
        CardStats CadieSC = new CardStats("CadieSC", 2, 2, 0, 2, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(CadieSC);
        CardStats TitanBooSC = new CardStats("TitanBooSC", 2, 2, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        cardstatcollection.Add(TitanBooSC);
        CardStats NuriSC = new CardStats("NuriSC", 1, 2, 2, 0, 0, 2, 0, 1, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(NuriSC);
        CardStats LuciaSC = new CardStats("LuciaSC", 1, 2, 2, 0, 0, 0, 0, 1, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(LuciaSC);
        CardStats ArthurSC = new CardStats("ArthurSC", 1, 2, 2, 0, 0, 1, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(ArthurSC);
        CardStats KazSC = new CardStats("KazSC", 1, 2, 2, 0, 0, 1, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(KazSC);
        CardStats SuccubusSC = new CardStats("SuccubusSC", 1, 3, 3, 0, 3, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(SuccubusSC);
        CardStats NellSC = new CardStats("NellSC", 1, 2, 2, 0, 0, 2, 1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(NellSC);
        CardStats SpikaSC = new CardStats("SpikaSC", 1, 2, 2, 0, 0, 2, 0, 3, 0, -2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(SpikaSC);
        CardStats PippinSC = new CardStats("PippinSC", 2, 2, 0, 2, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(PippinSC);
        CardStats MintySC = new CardStats("MintySC", 2, 2, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 33, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(MintySC);
        CardStats KoohSC = new CardStats("KoohSC", 1, 3, 3, 0, 1, 3, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(KoohSC);
        CardStats CeciliaSC = new CardStats("CeciliaSC", 1, 3, 3, 0, 1, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(CeciliaSC);
        CardStats MaxSC = new CardStats("MaxSC", 1, 2, 2, 0, 3, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(MaxSC);
        CardStats ArinSC = new CardStats("ArinSC", 1, 3, 3, 0, 0, 0, 2, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(ArinSC);
        CardStats RyoTohsakaSC = new CardStats("RyoTohsakaSC", 1, 3, 3, 0, 0, 2, 0, 1, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(RyoTohsakaSC);
        CardStats ArchSC = new CardStats("ArchSC", 1, 3, 3, 0, 1, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(ArchSC);
        CardStats KettoshiSC = new CardStats("KettoshiSC", 1, 3, 3, 0, 0, 0, 0, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(KettoshiSC);
        CardStats HanaSC = new CardStats("HanaSC", 1, 2, 2, 0, 0, 1, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(HanaSC);
        CardStats LilimSC = new CardStats("LilimSC", 1, 3, 3, 0, 0, 1, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(LilimSC);
        CardStats DominionSC = new CardStats("DominionSC", 1, 3, 3, 0, 0, 0, 0, 1, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(DominionSC);
        CardStats BaphometSC = new CardStats("BaphometSC", 1, 3, 3, 0, 0, 1, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(BaphometSC);
        CardStats KyungshiSC = new CardStats("KyungshiSC", 1, 3, 3, 0, 0, 2, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(KyungshiSC);
        CardStats SaberSC = new CardStats("SaberSC", 2, 3, 0, 3, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(SaberSC);
        CardStats TitanChamSC = new CardStats("TitanChamSC", 2, 3, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0);
        cardstatcollection.Add(TitanChamSC);
        CardStats BongdariSC = new CardStats("BongdariSC", 2, 3, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4);
        cardstatcollection.Add(BongdariSC);
        CardStats LoloSC = new CardStats("LoloSC", 2, 2, 0, 2, 0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(LoloSC);
        CardStats KumaSC = new CardStats("KumaSC", 2, 2, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(KumaSC);
        CardStats LukaSC = new CardStats("LukaSC", 2, 3, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 20, 0, 0, 0);
        cardstatcollection.Add(LukaSC);
        CardStats CamenSC = new CardStats("CamenSC", 2, 3, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 20, 0, 0);
        cardstatcollection.Add(CamenSC);
        CardStats MurenSC = new CardStats("MurenSC", 2, 2, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0);
        cardstatcollection.Add(MurenSC);
        CardStats CadieUL = new CardStats("CadieUL", 2, 4, 0, 4, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(CadieUL);
        CardStats KumaUL = new CardStats("KumaUL", 2, 4, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(KumaUL);
        CardStats TitanBooUL = new CardStats("TitanBooUL", 2, 4, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        cardstatcollection.Add(TitanBooUL);
        CardStats MintyUL = new CardStats("MintyUL", 2, 4, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 60, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(MintyUL);
        CardStats MaxUL = new CardStats("MaxUL", 1, 5, 5, 0, 5, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(MaxUL);
        CardStats PippinUL = new CardStats("PippinUL", 2, 4, 0, 4, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(PippinUL);
        CardStats KoohUL = new CardStats("KoohUL", 1, 5, 5, 0, 2, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(KoohUL);
        CardStats LoloUL = new CardStats("LoloUL", 2, 4, 0, 4, 0, 0, 0, 0, 0, 0, 12, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        cardstatcollection.Add(LoloUL);
        CardStats SCOre = new CardStats("SC-Ore", 3, 2, 1, 1, 1, 1, 1, 1, 1, 4, 8, 1, 33, 4, 1, 4, 20, 20, 1, 4);
        cardstatcollection.Add(SCOre);
        CardStats ULOre = new CardStats("UL-Ore", 3, 4, 4, 3, 3, 3, 3, 3, 3, 8, 12, 4, 60, 8, 2, 4, 50, 50, 6, 10);
        cardstatcollection.Add(ULOre);
        /////////////////////////////////////////////////
        //foreach (CardStats card in cardstatcollection)
        //{
        //    CardOption.;
        //}
    }
    #endregion
    #region BackboneLogics
    public static int[] GetIntArray(int num)
    {
        List<int> listOfInts = new List<int>();
        while (num > 0)
        {
            listOfInts.Add(num % 10);
            num = num / 10;
        }
        listOfInts.Reverse();
        return listOfInts.ToArray();
    }

    public static int WeightedRandomSelection(int[] weights, int totalWeight, Random rnd)
    {
        int randomValue = rnd.Next(totalWeight);
        int cumulativeWeight = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue < cumulativeWeight)
            {
                return i;
            }
        }
        return -1; // Should not happen if totalWeight is calculated correctly
    }
    public static void UpdateCharStat(RandomizedStatScore stats, int selection)
    {
        switch (selection)
        {
            case 0:
                stats.power++;
                break;
            case 1:
                stats.control++;
                break;
            case 2:
                stats.impact++;
                break;
            case 3:
                stats.spin++;
                break;
            case 4:
                stats.curve++;
                break;
            case 5:
                stats.speciallength++;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(selection), "Invalid selection index");
        }
    }

    public static void UpdateOtherStat(RandomizedStatScore stats, int selection)
    {
        switch (selection)
        {
            case 0:
                stats.speciallength++;
                break;
            case 1:
                stats.speciallengthgauge++;
                break;
            case 2:
                stats.pangyapx++;
                break;
            case 3:
                stats.initialguage++;
                break;
            case 4:
                stats.passivegauge++;
                break;
            case 5:
                stats.windreduction++;
                break;
            case 6:
                stats.pangyapxnpc++;
                break;
            case 7:
                stats.buffpangnpc++;
                break;
            case 8:
                stats.buffexpnpc++;
                break;
            case 9:
                stats.buffcontrolnpc++;
                break;
            case 10:
                stats.buffdroprate++;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(selection), "Invalid selection index");
        }
    }
    public static int ClampStatsTo24AndAccumulateExcess(int[] stats, int accumulatedPoints)
    {
        for (int i = 0; i < stats.Length; i++)
        {
            if (stats[i] > 24)
            {
                accumulatedPoints += stats[i] - 24;
                stats[i] = 24;
            }
        }

        return accumulatedPoints;
    }


    public static int[] GetTop3Indices(int[] stats)
    {
        if (stats == null || stats.Length == 0)
        {
            return new int[0];
        }

        var indexedStats = stats
            .Select((value, index) => new { Value = value, Index = index })
            .OrderByDescending(x => x.Value)
            .ToList();

        var topThreeValues = indexedStats.Take(3).Select(x => x.Value).Distinct().ToList();

        var indicesByValue = indexedStats
            .Where(x => topThreeValues.Contains(x.Value))
            .GroupBy(x => x.Value)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Index).ToList());

        var resultIndices = new List<int>();
        Random random = new Random();

        foreach (var value in topThreeValues)
        {
            if (indicesByValue.TryGetValue(value, out var indices))
            {
                var needed = 3 - resultIndices.Count;
                if (needed > 0)
                {
                    if (indices.Count <= needed)
                    {
                        resultIndices.AddRange(indices);
                    }
                    else
                    {
                        var selected = indices.OrderBy(x => random.Next()).Take(needed).ToList();
                        resultIndices.AddRange(selected);
                    }
                }
            }
            if (resultIndices.Count == 3)
            {
                break;
            }
        }

        return resultIndices.OrderBy(i => i).ToArray();
    }

    public static int AccumulateAndResetStats(int[] stats, int[] topIndices, int accumulatedPoints)
    {
        // Create a HashSet for faster lookups of top indices
        HashSet<int> topIndexSet = new HashSet<int>(topIndices);

        for (int i = 0; i < stats.Length; i++)
        {
            // Only process stats that are NOT in the top indices
            if (!topIndexSet.Contains(i))
            {
                accumulatedPoints += stats[i];
                stats[i] = 0;
            }
            // Stats at topIndices are neither accumulated nor reset.
            // The reason for this exclusion should be documented.
        }

        return accumulatedPoints;
    }


    public static void DistributePointsToTop3(int[] stats, int[] topIndices, int points, Random rnd)
    {
        List<int> eligibleIndices = topIndices.ToList();

        while (points > 0 && eligibleIndices.Count > 0)
        {
            // Filter out indices that have reached the limit
            eligibleIndices = eligibleIndices.Where(index => stats[index] < 24).ToList();

            if (eligibleIndices.Count == 0)
            {
                break; // No more eligible indices below the limit
            }

            int[] weights = eligibleIndices.Select(index => stats[index]).ToArray();
            int totalWeight = weights.Sum();

            if (totalWeight == 0)
            {
                break; // All eligible indices have a weight of 0 (shouldn't happen if values are < 24 and non-negative)
            }

            int selectedOffset = WeightedRandomSelection(weights, totalWeight, rnd);
            int statIndex = eligibleIndices[selectedOffset];

            stats[statIndex]++;
            points--;
        }
    }


    public static RandomizedStatScore TranslateStatsToText(RandomizedStatScore stats)
    {
        //CheckAllStats
        if (stats.power != 0)
        {
            if (stats.power >= 24)
                stats.stat.Add("Power +5");
            else if (stats.power >= 18)
                stats.stat.Add("Power +4");
            else if (stats.power >= 12)
                stats.stat.Add("Power +3");
            else if (stats.power >= 5)
                stats.stat.Add("Power +2");
            else
                stats.stat.Add("Power +1");
        }
        if (stats.control != 0)
        {
            if (stats.control >= 24)
                stats.stat.Add("Control +5");
            else if (stats.control >= 18)
                stats.stat.Add("Control +4");
            else if (stats.control >= 12)
                stats.stat.Add("Control +3");
            else if (stats.control >= 5)
                stats.stat.Add("Control +2");
            else
                stats.stat.Add("Control +1");
        }
        if (stats.impact != 0)
        {
            if (stats.impact >= 24)
            {
                Random rndnum = new Random();
                int mynumber = rndnum.Next(0, 2);
                if (mynumber == 1)
                    stats.stat.Add("+50% clubset experience");
                else
                    stats.stat.Add("75% chance to correct missed pangya shot.");
            }
            else if (stats.impact >= 18)
            {
                Random rndnum = new Random();
                int mynumber = rndnum.Next(0, 2);
                if (mynumber == 1)
                    stats.stat.Add("+20% clubset experience");
                else
                    stats.stat.Add("50% chance to correct missed pangya shot.");
            }
            else if (stats.impact >= 12)
            {
                Random rndnum = new Random();
                int mynumber = rndnum.Next(0, 2);
                if (mynumber == 1)
                    stats.stat.Add("+10% clubset experience");
                else
                    stats.stat.Add("35% chance to correct missed pangya shot.");
            }
            else if (stats.impact >= 5)
            {
                Random rndnum = new Random();
                int mynumber = rndnum.Next(0, 2);
                if (mynumber == 1)
                    stats.stat.Add("+5% clubset experience");
                else
                    stats.stat.Add("20% chance to correct missed pangya shot.");
            }
            else
                stats.stat.Add("10% chance to correct missed pangya shot.");
        }
        if (stats.spin > 0)
        {
            if (stats.spin >= 24)
                stats.stat.Add("Spin +5");
            else if (stats.spin >= 18)
                stats.stat.Add("Spin +4");
            else if (stats.spin >= 12)
                stats.stat.Add("Spin +3");
            else if (stats.spin >= 5)
                stats.stat.Add("Spin +2");
            else
                stats.stat.Add("Spin +1");
        }
        if (stats.curve > 0)
        {
            if (stats.curve >= 24)
                stats.stat.Add("Curve +5");
            else if (stats.curve >= 18)
                stats.stat.Add("Curve +4");
            else if (stats.curve >= 12)
                stats.stat.Add("Curve +3");
            else if (stats.curve >= 5)
                stats.stat.Add("Curve +2");
            else
                stats.stat.Add("Curve +1");
        }
        if (stats.speciallength > 0)
        {
            if (stats.speciallength >= 24)
                stats.stat.Add("+8 Yards [No control penalty!]");
            else if (stats.speciallength >= 18)
                stats.stat.Add("+4 Yards [No control penalty!]");
            else if (stats.speciallength >= 12)
                stats.stat.Add("+3 Yards [No control penalty!]");
            else if (stats.speciallength >= 5)
                stats.stat.Add("+2 Yards [No control penalty!]");
            else
                stats.stat.Add("+1 Yard [No control penalty!]");
        }
        if (stats.speciallengthgauge > 0)
        {
            if (stats.speciallengthgauge >= 24)
                stats.stat.Add("Special shot Length +12 Yards");
            else if (stats.speciallengthgauge >= 18)
                stats.stat.Add("Special shot Length +10 Yards");
            else if (stats.speciallengthgauge >= 12)
                stats.stat.Add("Special shot Length +8 Yards");
            else if (stats.speciallengthgauge >= 5)
                stats.stat.Add("Special shot Length +6 Yards");
            else
                stats.stat.Add("Special shot Length +4 Yards");
        }
        if (stats.pangyapx > 0)
        {
            if (stats.pangyapx >= 24)
                stats.stat.Add("Pangya bar +4 pixels");
            else if (stats.pangyapx >= 18)
                stats.stat.Add("Pangya bar +2 pixels");
            else if (stats.pangyapx >= 12)
                stats.stat.Add("Pangya bar +1.5 pixels");
            else if (stats.pangyapx >= 5)
                stats.stat.Add("Pangya bar +1 pixel");
            else
                stats.stat.Add("Pangya bar +0.5 pixel");
        }
        if (stats.initialguage > 0)
        {
            if (stats.initialguage >= 24)
                stats.stat.Add("Initial guage +60 points");
            else if (stats.initialguage >= 18)
                stats.stat.Add("Initial guage +33 points");
            else if (stats.initialguage >= 12)
                stats.stat.Add("Initial guage +24 points");
            else if (stats.initialguage >= 5)
                stats.stat.Add("Initial guage +15 points");
            else
                stats.stat.Add("Initial guage +10 points");
        }
        if (stats.passivegauge > 0)
        {
            if (stats.passivegauge >= 24)
                stats.stat.Add("Guage +6 points when hit pangya");
            else if (stats.passivegauge >= 18)
                stats.stat.Add("Guage +4 points when hit pangya");
            else if (stats.passivegauge >= 12)
                stats.stat.Add("Guage +3 points when hit pangya");
            else if (stats.passivegauge >= 5)
                stats.stat.Add("Guage +2 points when hit pangya");
            else
                stats.stat.Add("Guage +1 point when hit pangya");
        }
        if (stats.windreduction > 0)
        {
            //Random rndnum = new Random();
            //int mynumber = rndnum.Next(0, 2);
            if (stats.windreduction >= 24)
            {
                Random rndnum = new Random();
                int mynumber = rndnum.Next(0, 4);
                if (mynumber == 1)
                    stats.stat.Add("10% chance to reduce wind speed to 1m/s + 1/2,000 Chance to drop Azure Zepnhr artifact.");
                else if (mynumber == 2)
                    stats.stat.Add("10% chance to reduce wind speed to 1m/s + 4x chance to drop wind related artifact.");
                else
                    stats.stat.Add("Reduce all wind speed by 1.");
            }  
            else if (stats.windreduction >= 18)
                stats.stat.Add("7.5% chance to reduce wind speed to 1m/s");
            else if (stats.windreduction >= 12)
                stats.stat.Add("5% chance to reduce wind speed to 1m/s");
            else if (stats.windreduction >= 5)
                stats.stat.Add("5% chance to change wind direction to headwind/tailwind");
            else
                stats.stat.Add("3% chance to change wind direction to headwind/tailwind");
        }
        if (stats.pangyapxnpc > 0)
        {
            if (stats.pangyapxnpc >= 24)
                stats.stat.Add("Pangya bar +4 pixels, when club length is longer than 300y");
            else if (stats.pangyapxnpc >= 18)
                stats.stat.Add("Pangya bar +2 pixels, when club length is longer than 300y");
            else if (stats.pangyapxnpc >= 12)
                stats.stat.Add("Pangya bar +1.5 pixels, when club length is longer than 300y");
            else if (stats.pangyapxnpc >= 5)
                stats.stat.Add("Pangya bar +1 pixel, when club length is longer than 280y");
            else
                stats.stat.Add("Pangya bar +0.5 pixel, when club length is longer than 260y");
        }
        if (stats.buffpangnpc > 0)
        {
            if (stats.buffpangnpc >= 24)
                stats.stat.Add("Pang +20%");
            else if (stats.buffpangnpc >= 18)
                stats.stat.Add("Pang +15%");
            else if (stats.buffpangnpc >= 12)
                stats.stat.Add("Pang +10%");
            else if (stats.buffpangnpc >= 5)
                stats.stat.Add("Pang +5%");
            else
                stats.stat.Add("Pang +2%");
        }
        if (stats.buffexpnpc > 0)
        {
            if (stats.buffexpnpc >= 24)
                stats.stat.Add("Exp +20%");
            else if (stats.buffexpnpc >= 18)
                stats.stat.Add("Exp +15%");
            else if (stats.buffexpnpc >= 12)
                stats.stat.Add("Exp +10%");
            else if (stats.buffexpnpc >= 5)
                stats.stat.Add("Exp +5%");
            else
                stats.stat.Add("Exp +2%");
        }
        if (stats.buffcontrolnpc > 0)
        {
            if (stats.buffcontrolnpc >= 24)
                stats.stat.Add("Control +5, when club length is longer than 300y");
            else if (stats.buffcontrolnpc >= 18)
                stats.stat.Add("Control +4, when club length is longer than 300y");
            else if (stats.buffcontrolnpc >= 12)
                stats.stat.Add("Control +3, when club length is longer than 300y");
            else if (stats.buffcontrolnpc >= 5)
                stats.stat.Add("Control +2, when club length is longer than 280y");
            else
                stats.stat.Add("Control +1, when club length is longer than 260y");
        }
        if (stats.buffdroprate > 0)
        {
            if (stats.buffdroprate >= 24)
                stats.stat.Add("+5% Item drop rate.");
            else if (stats.buffdroprate >= 18)
                stats.stat.Add("+2.5% Item drop rate.");
            else if (stats.buffdroprate >= 12)
                stats.stat.Add("+1.5% Item drop rate.");
            else if (stats.buffdroprate >= 5)
                stats.stat.Add("+1% Item drop rate.");
            else
                stats.stat.Add("+0.5% Item drop rate.");
        }
        //Fill list if < 3 options
        while (stats.stat.Count < 3)
        {
            stats.stat.Add("-");
        }
        return stats;
    }
    #endregion
    #region FormsLogics
    
    private void AddCard(object sender, RoutedEventArgs e)
    {
        if (CardOption.SelectedItem == null)
        {
            MessageBox.Show("โปรดเลือกการ์ดก่อน");
            return;
        }
        if (selectedcard.Count >= 3)
        {
            MessageBox.Show("ใส่การ์ดเกิน 3 ใบไม่ได้");
            return;
        }
        else
        {
            ComboBoxItem selectedItem = CardOption.SelectedItem as ComboBoxItem;
            foreach (CardStats mycard in cardstatcollection)
            {
                if (mycard.cardname == selectedItem.Content.ToString())
                {
                    selectedcard.Add(mycard);
                    Label newLabel = new Label
                    {
                        Content = mycard.cardname,
                        Margin = new Thickness(5, 10, 5, 10), // Margin (left, top, right, bottom)
                        FontSize = 18,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                    };

                    // Add the Label to the StackPanel
                    ChosenCard.Children.Add(newLabel);
                    //MessageBox.Show("การ์ด " + mycard.cardname + " ได้ถูกใส่เรียบร้อยแล้ว");
                    break;
                }
            }
        }
    
    }

    private void RandStats(object sender, RoutedEventArgs e)
    {
        if (selectedcard.Count >= 4)
        {
            MessageBox.Show("ใส่การ์ดเกิน 3 ใบไม่ได้");
            //ResetB();
            return;
        }
        else if (selectedcard.Count <= 0)
        {
            MessageBox.Show("ใส่การ์ด 1-3 ใบ เพื่อดำเนินการ");
            //ResetB();
            return;
        }
        else
        {
            //สุ่มสถานะ
            AddCardButton.IsEnabled = false;
            RandomizeButton.IsEnabled = false;
            ResetButton.IsEnabled = false;
            Special_Clubset_Type1.IsEnabled = false;
            Special_Clubset_Type2.IsEnabled = false;
            Special_Clubset_Type3.IsEnabled = false;
            Calculateforrandomizing();
        }
    }

    private void Reset(object sender, RoutedEventArgs e)
    {
        ResetB();
    }
    private void ResetB()
    {
        selectedcard.Clear();
        if (ChosenCard.Children.Count > 0)
        {
            ChosenCard.Children.Clear();
        }
        Option1.Content = "-";
        Option2.Content = "-";
        Option3.Content = "-";
        AddCardButton.IsEnabled = true;
        RandomizeButton.IsEnabled = true;
        ResetButton.IsEnabled = true;
        Special_Clubset_Type1.IsEnabled = true;
        Special_Clubset_Type2.IsEnabled = true;
        Special_Clubset_Type3.IsEnabled = true;
        Special_Clubset_Type1.IsChecked = false;
        Special_Clubset_Type2.IsChecked = false;
        Special_Clubset_Type3.IsChecked = false;
        RandomizeCount = 0;
        RandCounterText.Content = RandomizeCount.ToString();
    }
    private void EnableButton()
    {
        AddCardButton.IsEnabled = true;
        RandomizeButton.IsEnabled = true;
        ResetButton.IsEnabled = true;
        Special_Clubset_Type1.IsEnabled = true;
        Special_Clubset_Type2.IsEnabled = true;
        Special_Clubset_Type3.IsEnabled = true;
    }
    private void Special_Clubset_Type1_Checked(object sender, RoutedEventArgs e)
    {
        if (Special_Clubset_Type1.IsChecked == true)
        {
            Special_Clubset_Type2.IsChecked = false;
            Special_Clubset_Type3.IsChecked = false;
        }
    }

    private void Special_Clubset_Type2_Checked(object sender, RoutedEventArgs e)
    {
        if (Special_Clubset_Type2.IsChecked == true)
        {
            Special_Clubset_Type1.IsChecked = false;
            Special_Clubset_Type3.IsChecked = false;
        }
    }
    private void Special_Clubset_Type3_Checked(object sender, RoutedEventArgs e)
    {
        if (Special_Clubset_Type3.IsChecked == true)
        {
            Special_Clubset_Type1.IsChecked = false;
            Special_Clubset_Type2.IsChecked = false;
        }
    }

    private void RemoveLastCard(object sender, RoutedEventArgs e)
    {
        if (selectedcard.Count >= 1)
        {
            selectedcard.RemoveAt(selectedcard.Count - 1);
            ChosenCard.Children.RemoveAt(ChosenCard.Children.Count - 1);
            return;
        }
    }

    private void ResetCount(object sender, RoutedEventArgs e)
    {
        RandomizeCount = 0;
        RandCounterText.Content = RandomizeCount.ToString();
    }
    #endregion

    
}