using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChartUtil
{
    [CreateAssetMenu(fileName = "ChartOptionsProfile", menuName = "EzChart/ChartOptionsProfile", order = 1)]
    public class ChartOptionsProfile : ScriptableObject
    {
        [System.Serializable]
        public struct ChartTextOptions
        {
            public bool color;
            public bool fontSize;
            public bool font;
            public bool customizedText;

            public void LoadPreset(ChartOptions.ChartTextOptions preset, ref ChartOptions.ChartTextOptions value)
            {
                if (color) value.color = preset.color;
                if (fontSize) value.fontSize = preset.fontSize;
                if (font) value.font = preset.font;
                if (customizedText) value.customizedText = preset.customizedText;
            }
        }

        [System.Serializable]
        public struct BarChartOptions
        {
            public bool colorByCategories;
            public bool barWidth;
            public bool itemSeparation;

            public void LoadPreset(ChartOptions.BarChartOptions preset, ref ChartOptions.BarChartOptions value)
            {
                if (colorByCategories) value.colorByCategories = preset.colorByCategories;
                if (barWidth) value.barWidth = preset.barWidth;
                if (itemSeparation) value.itemSeparation = preset.itemSeparation;
            }
        }

        [System.Serializable]
        public struct LineChartOptions
        {
            public bool pointSize;
            public bool enableLine;
            public bool lineWidth;
            public bool enableShade;
            public bool shadeTransparency;
            public bool enablePointOutline;
            public bool pointOutlineWidth;
            public bool pointOutlineColor;

            public void LoadPreset(ChartOptions.LineChartOptions preset, ref ChartOptions.LineChartOptions value)
            {
                if (pointSize) value.pointSize = preset.pointSize;
                if (enableLine) value.enableLine = preset.enableLine;
                if (lineWidth) value.lineWidth = preset.lineWidth;
                if (enableLine) value.enableLine = preset.enableLine;
                if (shadeTransparency) value.shadeTransparency = preset.shadeTransparency;
                if (enablePointOutline) value.enablePointOutline = preset.enablePointOutline;
                if (pointOutlineWidth) value.pointOutlineWidth = preset.pointOutlineWidth;
                if (pointOutlineColor) value.pointOutlineColor = preset.pointOutlineColor;
            }
        }

        [System.Serializable]
        public struct PieChartOptions
        {
            public bool itemSeparation;
            public bool innerSize;
            public bool outerSize;

            public void LoadPreset(ChartOptions.PieChartOptions preset, ref ChartOptions.PieChartOptions value)
            {
                if (itemSeparation) value.itemSeparation = preset.itemSeparation;
                if (innerSize) value.innerSize = preset.innerSize;
                if (outerSize) value.outerSize = preset.outerSize;
            }
        }

        [System.Serializable]
        public struct RoseChartOptions
        {
            public bool colorByCategories;
            public bool barWidth;
            public bool itemSeparation;
            public bool innerSize;
            public bool outerSize;

            public void LoadPreset(ChartOptions.RoseChartOptions preset, ref ChartOptions.RoseChartOptions value)
            {
                if (colorByCategories) value.colorByCategories = preset.colorByCategories;
                if (barWidth) value.barWidth = preset.barWidth;
                if (itemSeparation) value.itemSeparation = preset.itemSeparation;
                if (innerSize) value.innerSize = preset.innerSize;
                if (outerSize) value.outerSize = preset.outerSize;
            }
        }

        [System.Serializable]
        public struct RadarChartOptions
        {
            public bool circularGrid;
            public bool pointSize;
            public bool enableLine;
            public bool lineWidth;
            public bool enableShade;
            public bool shadeTransparency;
            public bool enablePointOutline;
            public bool pointOutlineWidth;
            public bool pointOutlineColor;
            public bool outerSize;

            public void LoadPreset(ChartOptions.RadarChartOptions preset, ref ChartOptions.RadarChartOptions value)
            {
                if (circularGrid) value.circularGrid = preset.circularGrid;
                if (pointSize) value.pointSize = preset.pointSize;
                if (enableLine) value.enableLine = preset.enableLine;
                if (lineWidth) value.lineWidth = preset.lineWidth;
                if (enableLine) value.enableLine = preset.enableLine;
                if (shadeTransparency) value.shadeTransparency = preset.shadeTransparency;
                if (enablePointOutline) value.enablePointOutline = preset.enablePointOutline;
                if (pointOutlineWidth) value.pointOutlineWidth = preset.pointOutlineWidth;
                if (pointOutlineColor) value.pointOutlineColor = preset.pointOutlineColor;
                if (outerSize) value.outerSize = preset.outerSize;
            }
        }

        [System.Serializable]
        public struct PlotOptions
        {
            public bool dataColor;
            public bool generalFont;
            public bool inverted;
            public bool reverseSeries;
            public bool mouseTracking;
            public bool columnStacking;
            public bool itemHighlightColor;
            public bool backgroundColor;
            public BarChartOptions barChartOption;
            public LineChartOptions lineChartOption;
            public PieChartOptions pieChartOption;
            public RoseChartOptions roseChartOption;
            public RadarChartOptions radarChartOption;

            public void LoadPreset(ChartOptions.PlotOptions preset, ref ChartOptions.PlotOptions value)
            {
                if (dataColor)
                {
                    value.dataColor = new Color[preset.dataColor.Length];
                    for (int i = 0; i < value.dataColor.Length; ++i) value.dataColor[i] = preset.dataColor[i];
                }
                if (generalFont) value.generalFont = preset.generalFont;
                if (inverted) value.inverted = preset.inverted;
                if (reverseSeries) value.reverseSeries = preset.reverseSeries;
                if (mouseTracking) value.mouseTracking = preset.mouseTracking;
                if (columnStacking) value.columnStacking = preset.columnStacking;
                if (itemHighlightColor) value.itemHighlightColor = preset.itemHighlightColor;
                if (backgroundColor) value.backgroundColor = preset.backgroundColor;
                barChartOption.LoadPreset(preset.barChartOption, ref value.barChartOption);
                lineChartOption.LoadPreset(preset.lineChartOption, ref value.lineChartOption);
                pieChartOption.LoadPreset(preset.pieChartOption, ref value.pieChartOption);
                roseChartOption.LoadPreset(preset.roseChartOption, ref value.roseChartOption);
                radarChartOption.LoadPreset(preset.radarChartOption, ref value.radarChartOption);
            }
        }

        [System.Serializable]
        public struct Title
        {
            public bool enableMainTitle;
            public bool mainTitle;
            public ChartTextOptions mainTitleOption;
            public bool enableSubTitle;
            public bool subTitle;
            public ChartTextOptions subTitleOption;

            public void LoadPreset(ChartOptions.Title preset, ref ChartOptions.Title value)
            {
                if (enableMainTitle) value.enableMainTitle = preset.enableMainTitle;
                if (mainTitle) value.mainTitle = preset.mainTitle;
                mainTitleOption.LoadPreset(preset.mainTitleOption, ref value.mainTitleOption);
                if (enableSubTitle) value.enableSubTitle = preset.enableSubTitle;
                if (subTitle) value.subTitle = preset.subTitle;
                subTitleOption.LoadPreset(preset.subTitleOption, ref value.subTitleOption);
            }
        }

        [System.Serializable]
        public struct XAxis
        {
            [Header("Grid Style")]
            public bool enableAxisLine;
            public bool axisLineColor;
            public bool axisLineWidth;
            public bool enableGridLine;
            public bool gridLineColor;
            public bool gridLineWidth;
            public bool enableTick;
            public bool tickColor;
            public bool tickSize;
            public bool autoAxisLinePosition;
            [Header("Title and labels")]
            public bool enableTitle;
            public bool title;
            public ChartTextOptions titleOption;
            public bool enableLabel;
            public ChartTextOptions labelOption;
            public bool autoRotateLabel;
            public bool skipLabel;
            public bool maxLabels;

            public void LoadPreset(ChartOptions.XAxis preset, ref ChartOptions.XAxis value)
            {
                if (enableAxisLine) value.enableAxisLine = preset.enableAxisLine;
                if (axisLineColor) value.axisLineColor = preset.axisLineColor;
                if (axisLineWidth) value.axisLineWidth = preset.axisLineWidth;
                if (enableGridLine) value.enableGridLine = preset.enableGridLine;
                if (gridLineColor) value.gridLineColor = preset.gridLineColor;
                if (gridLineWidth) value.gridLineWidth = preset.gridLineWidth;
                if (enableTick) value.enableTick = preset.enableTick;
                if (tickColor) value.tickColor = preset.tickColor;
                if (tickSize) value.tickSize = preset.tickSize;
                if (autoAxisLinePosition) value.autoAxisLinePosition = preset.autoAxisLinePosition;
                if (enableTitle) value.enableLabel = preset.enableLabel;
                if (title) value.title = preset.title;
                titleOption.LoadPreset(preset.titleOption, ref value.titleOption);
                if (enableLabel) value.enableLabel = preset.enableLabel;
                labelOption.LoadPreset(preset.labelOption, ref value.labelOption);
                if (autoRotateLabel) value.autoRotateLabel = preset.autoRotateLabel;
                if (skipLabel) value.skipLabel = preset.skipLabel;
                if (maxLabels) value.maxLabels = preset.maxLabels;
            }
        }

        [System.Serializable]
        public struct YAxis
        {
            [Header("Grid Style")]
            public bool enableAxisLine;
            public bool axisLineColor;
            public bool axisLineWidth;
            public bool enableGridLine;
            public bool gridLineColor;
            public bool gridLineWidth;
            public bool enableTick;
            public bool tickColor;
            public bool tickSize;
            [Header("Title and labels")]
            public bool enableTitle;
            public bool title;
            public ChartTextOptions titleOption;
            public bool enableLabel;
            public bool absoluteValue;
            public bool labelFormat;
            public ChartTextOptions labelOption;
            [Header("Range Setting")]
            public bool startFromZero;
            public bool minRangeDivision;
            public bool fixedRange;
            public bool fixedMinRange;
            public bool fixedMaxRange;
            public bool fixedRangeDivision;

            public void LoadPreset(ChartOptions.YAxis preset, ref ChartOptions.YAxis value)
            {
                if (enableAxisLine) value.enableAxisLine = preset.enableAxisLine;
                if (axisLineColor) value.axisLineColor = preset.axisLineColor;
                if (axisLineWidth) value.axisLineWidth = preset.axisLineWidth;
                if (enableGridLine) value.enableGridLine = preset.enableGridLine;
                if (gridLineColor) value.gridLineColor = preset.gridLineColor;
                if (gridLineWidth) value.gridLineWidth = preset.gridLineWidth;
                if (enableTick) value.enableTick = preset.enableTick;
                if (tickColor) value.tickColor = preset.tickColor;
                if (tickSize) value.tickSize = preset.tickSize;
                if (enableTitle) value.enableLabel = preset.enableLabel;
                if (title) value.title = preset.title;
                titleOption.LoadPreset(preset.titleOption, ref value.titleOption);
                if (enableLabel) value.enableLabel = preset.enableLabel;
                if (absoluteValue) value.absoluteValue = preset.absoluteValue;
                if (labelFormat) value.labelFormat = preset.labelFormat;
                labelOption.LoadPreset(preset.labelOption, ref value.labelOption);
                if (startFromZero) value.startFromZero = preset.startFromZero;
                if (minRangeDivision) value.minRangeDivision = preset.minRangeDivision;
                if (fixedRange) value.fixedRange = preset.fixedRange;
                if (fixedMinRange) value.fixedMinRange = preset.fixedMinRange;
                if (fixedMaxRange) value.fixedMaxRange = preset.fixedMaxRange;
                if (fixedRangeDivision) value.fixedRangeDivision = preset.fixedRangeDivision;
            }
        }

        [System.Serializable]
        public struct Tooltip
        {
            public bool enable;
            public bool share;
            public bool absoluteValue;
            public bool headerFormat;
            public bool pointFormat;
            public bool pointNumericFormat;
            public ChartTextOptions textOption;
            public bool backgroundColor;

            public void LoadPreset(ChartOptions.Tooltip preset, ref ChartOptions.Tooltip value)
            {
                if (enable) value.enable = preset.enable;
                if (share) value.share = preset.share;
                if (absoluteValue) value.absoluteValue = preset.absoluteValue;
                if (headerFormat) value.headerFormat = preset.headerFormat;
                if (pointFormat) value.pointFormat = preset.pointFormat;
                if (pointNumericFormat) value.pointNumericFormat = preset.pointNumericFormat;
                textOption.LoadPreset(preset.textOption, ref value.textOption);
                if (backgroundColor) value.backgroundColor = preset.backgroundColor;
            }
        }

        [System.Serializable]
        public struct Legend
        {
            public bool enable;
            public bool alignment;
            public bool itemLayout;
            public bool horizontalRows;
            public ChartTextOptions textOption;
            public bool iconImage;
            public bool backgroundColor;
            public bool highlightColor;
            public bool dimmedColor;

            public void LoadPreset(ChartOptions.Legend preset, ref ChartOptions.Legend value)
            {
                if (enable) value.enable = preset.enable;
                if (alignment) value.alignment = preset.alignment;
                if (itemLayout) value.itemLayout = preset.itemLayout;
                if (horizontalRows) value.horizontalRows = preset.horizontalRows;
                textOption.LoadPreset(preset.textOption, ref value.textOption);
                if (iconImage) value.iconImage = preset.iconImage;
                if (backgroundColor) value.backgroundColor = preset.backgroundColor;
                if (highlightColor) value.highlightColor = preset.highlightColor;
                if (dimmedColor) value.dimmedColor = preset.dimmedColor;
            }
        }

        [System.Serializable]
        public struct Label
        {
            public bool enable;
            public bool absoluteValue;
            public bool format;
            public bool numericFormat;
            public ChartTextOptions textOption;
            public bool anchoredPosition;
            public bool offset;
            public bool rotation;
            public bool bestFit;

            public void LoadPreset(ChartOptions.Label preset, ref ChartOptions.Label value)
            {
                if (enable) value.enable = preset.enable;
                if (absoluteValue) value.absoluteValue = preset.absoluteValue;
                if (format) value.format = preset.format;
                if (numericFormat) value.numericFormat = preset.numericFormat;
                textOption.LoadPreset(preset.textOption, ref value.textOption);
                if (anchoredPosition) value.anchoredPosition = preset.anchoredPosition;
                if (offset) value.offset = preset.offset;
                if (rotation) value.rotation = preset.rotation;
                if (bestFit) value.bestFit = preset.bestFit;
            }
        }

        public PlotOptions plotOptions;
        public Title title;
        public XAxis xAxis;
        public YAxis yAxis;
        public Tooltip tooltip;
        public Legend legend;
        public Label label;

        public void LoadPreset(ChartOptionsPreset preset, ref ChartOptions value)
        {
            plotOptions.LoadPreset(preset.plotOptions, ref value.plotOptions);
            title.LoadPreset(preset.title, ref value.title);
            xAxis.LoadPreset(preset.xAxis, ref value.xAxis);
            yAxis.LoadPreset(preset.yAxis, ref value.yAxis);
            tooltip.LoadPreset(preset.tooltip, ref value.tooltip);
            legend.LoadPreset(preset.legend, ref value.legend);
            label.LoadPreset(preset.label, ref value.label);
        }

        public void LoadPreset(ChartOptions preset, ref ChartOptions value)
        {
            plotOptions.LoadPreset(preset.plotOptions, ref value.plotOptions);
            title.LoadPreset(preset.title, ref value.title);
            xAxis.LoadPreset(preset.xAxis, ref value.xAxis);
            yAxis.LoadPreset(preset.yAxis, ref value.yAxis);
            tooltip.LoadPreset(preset.tooltip, ref value.tooltip);
            legend.LoadPreset(preset.legend, ref value.legend);
            label.LoadPreset(preset.label, ref value.label);
        }
    }
}