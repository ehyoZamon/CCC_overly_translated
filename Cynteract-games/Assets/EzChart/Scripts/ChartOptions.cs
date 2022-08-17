using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if CHART_TMPRO
using TMPro;
#endif

namespace ChartUtil
{
    public enum ChartType
    {
        PieChart, BarChart, LineChart, RoseChart, RadarChart
    }

    public enum ColumnStacking
    {
        None, Normal, Percent
    }

    public class ChartOptions : MonoBehaviour
    {
        [System.Serializable]
        public struct ChartTextOptions
        {
            [Tooltip("Text color")]
            public Color color;
            [Tooltip("Text font size")]
            public int fontSize;
#if CHART_TMPRO
            [Tooltip("Text font. If this is null, Options - Plot Option - General Font will be used")]
            public TMP_FontAsset font;
            [Tooltip("Text template. Chart will instantiate the text GameObject with all its attached components (e.g. shadow, outline), which allows more advanced text settings. This will overwrite all basic text options (Color, Font Size and Font).")]
            public TextMeshProUGUI customizedText;
            public ChartTextOptions(Color c, TMP_FontAsset f, int fs, TextMeshProUGUI ct = null)
#else
            [Tooltip("Text font. If this is null, Options - Plot Option - General Font will be used")]
            public Font font;
            [Tooltip("Text template. Chart will instantiate the text GameObject with all its attached components (e.g. shadow, outline), which allows more advanced text settings. This will overwrite all basic text options (Color, Font Size and Font).")]
            public Text customizedText;
            public ChartTextOptions(Color c, Font f, int fs, Text ct = null)
#endif
            {
                color = c;
                font = f;
                fontSize = fs;
                customizedText = ct;
            }
        }

        [System.Serializable]
        public class BarChartOptions
        {
            [Tooltip("Set data color by categories instead of by series")]
            public bool colorByCategories = false;
            [Tooltip("Width of bars")]
            public float barWidth = 10.0f;
            [Tooltip("Separation distance between bars")]
            public float itemSeparation = 3.0f;
        }

        [System.Serializable]
        public class LineChartOptions
        {
            [Tooltip("Point size for line chart item points")]
            public float pointSize = 10.0f;
            [Tooltip("Enable/disable lines")]
            public bool enableLine = true;
            [Tooltip("Line width for line chart lines")]
            public float lineWidth = 5.0f;
            [Tooltip("Enable/disable shade under the lines")]
            public bool enableShade = false;
            [Tooltip("Transparency of the shade")]
            public float shadeTransparency = 0.5f;
            [Tooltip("Enable/disable point outline")]
            public bool enablePointOutline = false;
            [Tooltip("Width of point outline")]
            public float pointOutlineWidth = 1.0f;
            [Tooltip("Color of point outline")]
            public Color pointOutlineColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
        }

        [System.Serializable]
        public class PieChartOptions
        {
            [Tooltip("Separation distance between items")]
            public float itemSeparation = 0.0f;
            [Tooltip("Inner size of the chart")]
            [Range(0.0f, 1.0f)] public float innerSize = 0.0f;
            [Tooltip("Outer size of the chart")]
            [Range(0.0f, 1.0f)] public float outerSize = 1.0f;
        }

        [System.Serializable]
        public class RoseChartOptions
        {
            [Tooltip("Set data color by categories instead of by series")]
            public bool colorByCategories = false;
            [Tooltip("Width of bars")]
            public float barWidth = 10.0f;
            [Tooltip("Separation distance between bars")]
            public float itemSeparation = 3.0f;
            [Tooltip("Inner size of the chart")]
            [Range(0.0f, 1.0f)] public float innerSize = 0.0f;
            [Tooltip("Outer size of the chart")]
            [Range(0.0f, 1.0f)] public float outerSize = 1.0f;
        }

        [System.Serializable]
        public class RadarChartOptions
        {
            [Tooltip("Use circular grid")]
            public bool circularGrid = false;
            [Tooltip("Point size for radar chart item points")]
            public float pointSize = 10.0f;
            [Tooltip("Enable/disable lines")]
            public bool enableLine = true;
            [Tooltip("Line width for radar chart lines")]
            public float lineWidth = 5.0f;
            [Tooltip("Enable/disable shade")]
            public bool enableShade = false;
            [Tooltip("Transparency of the shade")]
            public float shadeTransparency = 0.5f;
            [Tooltip("Enable/disable point outline")]
            public bool enablePointOutline = false;
            [Tooltip("Width of point outline")]
            public float pointOutlineWidth = 1.0f;
            [Tooltip("Color of point outline")]
            public Color pointOutlineColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
            [Tooltip("Outer size of the chart")]
            [Range(0.0f, 1.0f)] public float outerSize = 1.0f;
        }

        [System.Serializable]
        public class PlotOptions
        {
            [Tooltip("Colors for chart series data, if number of series is larger then data color length, it will loop over the first color element")]
            public Color[] dataColor = new Color[11]
            {
                new Color32 (125, 180, 240, 255),
                new Color32 (255, 125, 80, 255),
                new Color32 (144, 237, 125, 255),
                new Color32 (247, 163, 92, 255),
                new Color32 (128, 133, 233, 255),
                new Color32 (241, 92, 128, 255),
                new Color32 (228, 211, 84, 255),
                new Color32 (43, 144, 143, 255),
                new Color32 (244, 91, 91, 255),
                new Color32 (190, 110, 240, 255),
                new Color32 (170, 240, 240, 255)
            };

#if CHART_TMPRO
            [Tooltip("Font used for the all text elements in the chart")]
            public TMP_FontAsset generalFont = null;
#else
            [Tooltip("Font used for the all text elements in the chart")]
            public Font generalFont = null;
#endif
            [Tooltip("Invert XY axes (if applicable)")]
            public bool inverted = false;
            [Tooltip("Reverse series display order")]
            public bool reverseSeries = false;
            [Tooltip("Track mouse position to highlight chart items and display tooltip")]
            public bool mouseTracking = true;
            [Tooltip("Column stacking modes")]
            public ColumnStacking columnStacking = ColumnStacking.None;
            [Tooltip("Item background color when mouse is hovering the item")]
            public Color itemHighlightColor = new Color32(173, 219, 238, 100);
            [Tooltip("Chart background color")]
            public Color backgroundColor = Color.clear;
            public BarChartOptions barChartOption = new BarChartOptions();
            public LineChartOptions lineChartOption = new LineChartOptions();
            public PieChartOptions pieChartOption = new PieChartOptions();
            public RoseChartOptions roseChartOption = new RoseChartOptions();
            public RadarChartOptions radarChartOption = new RadarChartOptions();
        }
        
        [System.Serializable]
        public class Title
        {
            [Tooltip("Show/hide chart main title")]
            public bool enableMainTitle = true;
            [Tooltip("Main title content")]
            public string mainTitle = "Main Title";
            [Tooltip("Main title text options")]
            public ChartTextOptions mainTitleOption = new ChartTextOptions(new Color(0.2f, 0.2f, 0.2f, 1.0f), null, 18);
            [Tooltip("Show/hide chart sub title")]
            public bool enableSubTitle = false;
            [Tooltip("Sub title content")]
            public string subTitle = "Sub Title";
            [Tooltip("Sub title text options")]
            public ChartTextOptions subTitleOption = new ChartTextOptions(new Color(0.2f, 0.2f, 0.2f, 1.0f), null, 12);
        }

        [System.Serializable]
        public class XAxis
        {
            [Header("Grid Style")]
            [Tooltip("Show/Hide axis line")]
            public bool enableAxisLine = true;
            [Tooltip("Color of axis line")]
            public Color axisLineColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
            [Tooltip("Width of axis line")]
            public float axisLineWidth = 2;
            [Tooltip("Show/Hide grid lines")]
            public bool enableGridLine = true;
            [Tooltip("Color of grid lines")]
            public Color gridLineColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
            [Tooltip("Width of grid lines")]
            public float gridLineWidth = 1;
            [Tooltip("Show/Hide ticks")]
            public bool enableTick = true;
            [Tooltip("Color of ticks")]
            public Color tickColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
            [Tooltip("Width/Length of ticks")]
            public Vector2 tickSize = new Vector2(2.0f, 4.0f);
            [Tooltip("Auto adjust axis line position so it always indicates '0'")]
            public bool autoAxisLinePosition = false;

            [Header("Title and labels")]
            [Tooltip("Show/hide x axis title")]
            public bool enableTitle = false;
            [Tooltip("X axis title content")]
            public string title = "xAxis";
            [Tooltip("Title text options")]
            public ChartTextOptions titleOption = new ChartTextOptions(new Color(0.2f, 0.2f, 0.2f, 1.0f), null, 14);
            [Tooltip("Show/hide x axis labels")]
            public bool enableLabel = true;
            [Tooltip("Label text options")]
            public ChartTextOptions labelOption = new ChartTextOptions(new Color(0.2f, 0.2f, 0.2f, 1.0f), null, 12);
            [Tooltip("Auto rotate labels when labels are too long")]
            public bool autoRotateLabel = true;
            [Tooltip("Skip labels to prevent overlapping. '-1' indicates auto skipping. '0' indicates that no label will be skipped. ")]
            public int skipLabel = -1;
            [Tooltip("Maximum number of labels. '0' indicates unrestricted number. Only applicable when auto skipping is enabled (skipLabel = -1).")]
            public int maxLabels = 0;
        }

        [System.Serializable]
        public class YAxis
        {
            [Header("Grid Style")]
            [Tooltip("Show/Hide axis line")]
            public bool enableAxisLine = true;
            [Tooltip("Color of axis line")]
            public Color axisLineColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
            [Tooltip("Width of axis line")]
            public float axisLineWidth = 2;
            [Tooltip("Show/Hide grid lines")]
            public bool enableGridLine = false;
            [Tooltip("Color of grid lines")]
            public Color gridLineColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
            [Tooltip("Width of grid lines")]
            public float gridLineWidth = 1;
            [Tooltip("Show/Hide ticks")]
            public bool enableTick = false;
            [Tooltip("Color of ticks")]
            public Color tickColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
            [Tooltip("Width/Length of ticks")]
            public Vector2 tickSize = new Vector2(2.0f, 4.0f);

            [Header("Title and labels")]
            [Tooltip("Show/hide y axis title")]
            public bool enableTitle = false;
            [Tooltip("Y axis title content")]
            public string title = "yAxis";
            [Tooltip("Title text options")]
            public ChartTextOptions titleOption = new ChartTextOptions(new Color(0.2f, 0.2f, 0.2f, 1.0f), null, 14);
            [Tooltip("Show/hide y axis labels")]
            public bool enableLabel = true;
            [Tooltip("Display absolute label values")]
            public bool absoluteValue = false;
            [Tooltip("Label format string, keywords will be replaced while other characters remain the same, useful for adding unit. " +
                "'{value}' will be replaced with label value. ")]
            public string labelFormat = "{value}";
            [Tooltip("Label text options")]
            public ChartTextOptions labelOption = new ChartTextOptions(new Color(0.2f, 0.2f, 0.2f, 1.0f), null, 12);

            [Header("Range Setting")]
            [Tooltip("Y axis value range always starts from zero. If disabled, the range will focus between the minimum value and maximum value")]
            public bool startFromZero = true;
            [Tooltip("Min number of divisions for the y axis value range")]
            public int minRangeDivision = 4;
            [Tooltip("Manually control y axis range. If fixed range is enabled, 'StartFromZero' and 'MinRangeDivision' will be overwritten by fixed range options")]
            public bool fixedRange = false;
            [Tooltip("Min value of fixed range")]
            public float fixedMinRange = 0.0f;
            [Tooltip("Max value of fixed range")]
            public float fixedMaxRange = 100.0f;
            [Tooltip("Number of divisions")]
            public int fixedRangeDivision = 4;
        }

        [System.Serializable]
        public class Tooltip
        {
            [Tooltip("Enable/disable tooltip when mouse is hovering chart items")]
            public bool enable = true;
            [Tooltip("Share tooltip for all series in current category or display tooltip for individual series")]
            public bool share = true;
            [Tooltip("Display absolute data values")]
            public bool absoluteValue = false;
            [Tooltip("Tooltip header format string, keywords will be replaced while other characters remain the same. " +
                "'{category}' will be replaced with current category.")]
            public string headerFormat = "{category}";
            [Tooltip("Tooltip point format string, keywords will be replaced while other characters remain the same. " +
                "'{series.name}' will be replaced with series name. " +
                "'{data.value}' will be replaced with data value. " +
                "'{data.percentage}' will be replaced with data percentage in current category. ")]
            public string pointFormat = "{series.name}: {data.value}";
            [Tooltip("Tooltip point numeric format string, it is a C# standard numeric format string. Leave it empty for auto numeric format")]
            public string pointNumericFormat = "";
            [Tooltip("Tooltip text options")]
            public ChartTextOptions textOption = new ChartTextOptions(new Color(0.9f, 0.9f, 0.9f, 1.0f), null, 14);
            [Tooltip("Color of tooltip background")]
            public Color backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.7f);
        }

        [System.Serializable]
        public class Legend
        {
            [Tooltip("Show/hide chart legends")]
            public bool enable = true;
            [Tooltip("Legend alignment position")]
            public TextAnchor alignment = TextAnchor.LowerCenter;
            [Tooltip("Horizontal or vertical layout")]
            public RectTransform.Axis itemLayout = RectTransform.Axis.Horizontal;
            [Tooltip("Number of rows for horizontal layout")]
            public int horizontalRows = 1;
            [Tooltip("Legend text options")]
            public ChartTextOptions textOption = new ChartTextOptions(new Color(0.2f, 0.2f, 0.2f, 1.0f), null, 14);
            [Tooltip("Legend icon sprite image")]
            public Sprite iconImage = null;
            [Tooltip("Color of legend background")]
            public Color backgroundColor = Color.clear;
            [Tooltip("Color when legend is highlighted")]
            public Color highlightColor = new Color(0.8f, 0.8f, 0.8f, 0.7f);
            [Tooltip("Color when legend is turned off")]
            public Color dimmedColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        }

        [System.Serializable]
        public class Label
        {
            [Tooltip("Enable/disable label of chart data")]
            public bool enable = false;
            [Tooltip("Display absolute data values")]
            public bool absoluteValue = false;
            [Tooltip("Label format string, keywords will be replaced while other characters remain the same. " +
                "'{series.name}' will be replaced with series name. " +
                "'{data.value}' will be replaced with data value. " +
                "'{data.percentage}' will be replaced with data percentage in current category. ")]
            public string format = "{data.value}";
            [Tooltip("Label numeric format string, it is a C# standard numeric format string. Leave it empty for auto numeric format")]
            public string numericFormat = "";
            [Tooltip("Label text options")]
            public ChartTextOptions textOption = new ChartTextOptions(new Color(0.2f, 0.2f, 0.2f, 1.0f), null, 14);
            [Tooltip("Label anchored position in the chart item, 0.0/0.5/1.0 indicates beginning/middle/end of the item")]
            public float anchoredPosition = 1.0f;
            [Tooltip("Label offset distance from the chart item, positive/negative value will move label away/toward the chart center")]
            public float offset = 12.0f;
            [Tooltip("Label rotation")]
            public float rotation = 0.0f;
            [Tooltip("Adjust pie chart size to fit with labels (only applicable for pie chart)")]
            public bool bestFit = true;
        }

        public PlotOptions plotOptions = new PlotOptions();
        public Title title = new Title();
        public XAxis xAxis = new XAxis();
        public YAxis yAxis = new YAxis();
        public Tooltip tooltip = new Tooltip();
        public Legend legend = new Legend();
        public Label label = new Label();

        private void Reset()
        {
#if CHART_TMPRO
            plotOptions.generalFont = Resources.Load("Fonts & Materials/LiberationSans SDF", typeof(TMP_FontAsset)) as TMP_FontAsset;
#else
            plotOptions.generalFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
#endif
            legend.iconImage = Resources.Load<Sprite>("Images/Chart_Circle_128x128");
        }
    }
}
