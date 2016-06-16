#region << 版 本 注 释 >>
/*
     * ========================================================================
     * Copyright Notice © 2010-2014 TideBuy.com All rights reserved .
     * ========================================================================
     * 机器名称：USER-429236GLDJ 
     * 文件名：  ThumbnailsImageHandler 
     * 版本号：  V1.0.0.0 
     * 创建人：  wyp 
     * 创建时间：2014/8/27 10:00:45 
     * 描述    : 压缩图片类型
     * =====================================================================
     * 修改时间：2014/8/27 10:00:45 
     * 修改人  ：  
     * 版本号  ： V1.0.0.0 
     * 描述    ：
*/
#endregion
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System;
using System.Collections;


namespace TLZ.Helper
{
    /// <summary>
    /// 压缩图片类型
    /// </summary>
    public class ThumbnailsImageHandler
    {
        #region 正方型裁剪并缩放
        /// <summary>
        /// 正方型裁剪
        /// 以图片中心为轴心，截取正方型，然后等比缩放
        /// 用于头像处理
        /// </summary>
        /// <param name="fromFile">原图Stream对象</param>
        /// <param name="fileSavePath">缩略图存放地址</param>
        /// <param name="side">指定的边长（正方型）</param>
        /// <param name="quality">质量（范围0-100）</param>
        public static void CutForSquare(System.IO.FileStream fromFile, string fileSavePath, int side, int quality)
        {
            string dir = IOHelper.GetDirectoryName(fileSavePath);
            IOHelper.CreateDirectory(dir);
            IOHelper.DeleteFile(fileSavePath);

            ImageCodecInfo ici;
            EncoderParameters ep;
            ImageEnoders(fromFile, quality, out ici, out ep);

            //原始图片（获取原始图片创建对象，并使用流中嵌入的颜色管理信息）
            System.Drawing.Image initImage = System.Drawing.Image.FromStream(fromFile, true);

            //原图宽高均小于模版，不作处理，直接保存
            if (initImage.Width <= side && initImage.Height <= side)
            {
                initImage.Save(fileSavePath, ici, ep);

                //initImage.Save(fileSavePath, initImage.RawFormat);
            }
            else
            {
                //原始图片的宽、高
                int initWidth = initImage.Width;
                int initHeight = initImage.Height;

                //非正方型先裁剪为正方型
                if (initWidth != initHeight)
                {
                    //截图对象
                    System.Drawing.Image pickedImage = null;
                    System.Drawing.Graphics pickedG = null;

                    //宽大于高的横图
                    if (initWidth > initHeight)
                    {
                        //对象实例化
                        pickedImage = new System.Drawing.Bitmap(initHeight, initHeight);
                        pickedG = System.Drawing.Graphics.FromImage(pickedImage);
                        //设置质量
                        SetGraphicsMode(pickedG);

                        //定位
                        Rectangle fromR = new Rectangle((initWidth - initHeight) / 2, 0, initHeight, initHeight);
                        Rectangle toR = new Rectangle(0, 0, initHeight, initHeight);
                        //画图
                        pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);
                        //重置宽
                        initWidth = initHeight;
                    }
                    //高大于宽的竖图
                    else
                    {
                        //对象实例化
                        pickedImage = new System.Drawing.Bitmap(initWidth, initWidth);
                        pickedG = System.Drawing.Graphics.FromImage(pickedImage);
                        //设置质量
                        SetGraphicsMode(pickedG);

                        //定位
                        Rectangle fromR = new Rectangle(0, (initHeight - initWidth) / 2, initWidth, initWidth);
                        Rectangle toR = new Rectangle(0, 0, initWidth, initWidth);
                        //画图
                        pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);
                        //重置高
                        initHeight = initWidth;
                    }

                    //将截图对象赋给原图
                    initImage = (System.Drawing.Image)pickedImage.Clone();
                    //释放截图资源
                    pickedG.Dispose();
                    pickedImage.Dispose();
                }

                //缩略图对象
                System.Drawing.Image resultImage = new System.Drawing.Bitmap(side, side);
                System.Drawing.Graphics resultG = System.Drawing.Graphics.FromImage(resultImage);
                //设置质量
                SetGraphicsMode(resultG);

                //用指定背景色清空画布
                resultG.Clear(Color.White);
                //绘制缩略图
                resultG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, side, side), new System.Drawing.Rectangle(0, 0, initWidth, initHeight), System.Drawing.GraphicsUnit.Pixel);

                //保存缩略图
                resultImage.Save(fileSavePath, ici, ep);

                //resultImage.Save(fileSavePath, initImage.RawFormat);

                //释放关键质量控制所用资源
                ep.Dispose();

                //释放缩略图资源
                resultG.Dispose();
                resultImage.Dispose();

                //释放原始图片资源
                initImage.Dispose();
            }
        }
        /// <summary>
        /// 正方型裁剪
        /// 以图片中心为轴心，截取正方型，然后等比缩放
        /// 用于头像处理
        /// </summary>
        /// <param name="fromFile">原图Stream对象</param>
        /// <param name="side">指定的边长（正方型）</param>
        /// <param name="quality">质量（范围0-100）</param>
        /// <returns>缩略图</returns>
        public static System.Drawing.Image CutForSquare(System.IO.FileStream fromFile, int side, int quality, out ImageCodecInfo ici, out EncoderParameters ep)
        {
            ici = null;
            ep = null;
            ImageEnoders(fromFile, quality, out ici, out ep);

            //原始图片（获取原始图片创建对象，并使用流中嵌入的颜色管理信息）
            System.Drawing.Image initImage = System.Drawing.Image.FromStream(fromFile, true);

            //原图宽高均小于模版，不作处理，直接保存
            if (initImage.Width <= side && initImage.Height <= side)
            {
                return initImage;
            }
            else
            {
                //原始图片的宽、高
                int initWidth = initImage.Width;
                int initHeight = initImage.Height;

                //非正方型先裁剪为正方型
                if (initWidth != initHeight)
                {
                    //截图对象
                    System.Drawing.Image pickedImage = null;
                    System.Drawing.Graphics pickedG = null;

                    //宽大于高的横图
                    if (initWidth > initHeight)
                    {
                        //对象实例化
                        pickedImage = new System.Drawing.Bitmap(initHeight, initHeight);
                        pickedG = System.Drawing.Graphics.FromImage(pickedImage);
                        //设置质量
                        SetGraphicsMode(pickedG);

                        //定位
                        Rectangle fromR = new Rectangle((initWidth - initHeight) / 2, 0, initHeight, initHeight);
                        Rectangle toR = new Rectangle(0, 0, initHeight, initHeight);
                        //画图
                        pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);
                        //重置宽
                        initWidth = initHeight;
                    }
                    //高大于宽的竖图
                    else
                    {
                        //对象实例化
                        pickedImage = new System.Drawing.Bitmap(initWidth, initWidth);
                        pickedG = System.Drawing.Graphics.FromImage(pickedImage);
                        //设置质量
                        SetGraphicsMode(pickedG);

                        //定位
                        Rectangle fromR = new Rectangle(0, (initHeight - initWidth) / 2, initWidth, initWidth);
                        Rectangle toR = new Rectangle(0, 0, initWidth, initWidth);
                        //画图
                        pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);
                        //重置高
                        initHeight = initWidth;
                    }

                    //将截图对象赋给原图
                    initImage = (System.Drawing.Image)pickedImage.Clone();
                    //释放截图资源
                    pickedG.Dispose();
                    pickedImage.Dispose();
                }

                //缩略图对象
                System.Drawing.Image resultImage = new System.Drawing.Bitmap(side, side);
                System.Drawing.Graphics resultG = System.Drawing.Graphics.FromImage(resultImage);
                //设置质量
                SetGraphicsMode(resultG);

                //用指定背景色清空画布
                resultG.Clear(Color.White);
                //绘制缩略图
                resultG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, side, side), new System.Drawing.Rectangle(0, 0, initWidth, initHeight), System.Drawing.GraphicsUnit.Pixel);

                //释放缩略图资源
                resultG.Dispose();

                //释放原始图片资源
                initImage.Dispose();

                //保存缩略图
                return resultImage;
            }
        }
        #endregion

        #region 自定义裁剪并缩放
        /// <summary>
        /// 指定长宽裁剪
        /// 按模版比例最大范围的裁剪图片并缩放至模版尺寸
        /// </summary>
        /// <param name="fromFile">原图Stream对象</param>
        /// <param name="fileSavePath">缩略图存放地址</param>
        /// <param name="maxWidth">最大宽(单位:px)</param>
        /// <param name="maxHeight">最大高(单位:px)</param>
        /// <param name="quality">质量（范围0-100）</param>
        public static void CutForRectangle(System.IO.FileStream fromFile, string fileSavePath, int maxWidth, int maxHeight, int quality)
        {
            string dir = IOHelper.GetDirectoryName(fileSavePath);
            IOHelper.CreateDirectory(dir);
            IOHelper.DeleteFile(fileSavePath);

            ImageCodecInfo ici;
            EncoderParameters ep;
            ImageEnoders(fromFile, quality, out ici, out ep);

            //从文件获取原始图片，并使用流中嵌入的颜色管理信息
            System.Drawing.Image initImage = System.Drawing.Image.FromStream(fromFile, true);

            //原图宽高均小于模版，不作处理，直接保存
            if (initImage.Width <= maxWidth && initImage.Height <= maxHeight)
            {
                initImage.Save(fileSavePath, ici, ep);

                //initImage.Save(fileSavePath, initImage.RawFormat);
            }
            else
            {
                //模版的宽高比例
                double templateRate = (double)maxWidth / maxHeight;
                //原图片的宽高比例
                double initRate = (double)initImage.Width / initImage.Height;

                //原图与模版比例相等，直接缩放
                if (templateRate == initRate)
                {
                    //按模版大小生成最终图片
                    System.Drawing.Image templateImage = new System.Drawing.Bitmap(maxWidth, maxHeight);
                    System.Drawing.Graphics templateG = System.Drawing.Graphics.FromImage(templateImage);
                    //设置图片质量
                    SetGraphicsMode(templateG);

                    templateG.Clear(Color.White);
                    templateG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, maxWidth, maxHeight), new System.Drawing.Rectangle(0, 0, initImage.Width, initImage.Height), System.Drawing.GraphicsUnit.Pixel);

                    //保存缩略图
                    templateImage.Save(fileSavePath, ici, ep);

                    //templateImage.Save(fileSavePath, initImage.RawFormat);
                }
                //原图与模版比例不等，裁剪后缩放
                else
                {
                    //裁剪对象
                    System.Drawing.Image pickedImage = null;
                    System.Drawing.Graphics pickedG = null;

                    //定位
                    Rectangle fromR = new Rectangle(0, 0, 0, 0);//原图裁剪定位
                    Rectangle toR = new Rectangle(0, 0, 0, 0);//目标定位

                    //宽为标准进行裁剪
                    if (templateRate > initRate)
                    {
                        //裁剪对象实例化
                        pickedImage = new System.Drawing.Bitmap(initImage.Width, (int)System.Math.Floor(initImage.Width / templateRate));
                        pickedG = System.Drawing.Graphics.FromImage(pickedImage);

                        //裁剪源定位
                        fromR.X = 0;
                        fromR.Y = (int)System.Math.Floor((initImage.Height - initImage.Width / templateRate) / 2);
                        fromR.Width = initImage.Width;
                        fromR.Height = (int)System.Math.Floor(initImage.Width / templateRate);

                        //裁剪目标定位
                        toR.X = 0;
                        toR.Y = 0;
                        toR.Width = initImage.Width;
                        toR.Height = (int)System.Math.Floor(initImage.Width / templateRate);
                    }
                    //高为标准进行裁剪
                    else
                    {
                        pickedImage = new System.Drawing.Bitmap((int)System.Math.Floor(initImage.Height * templateRate), initImage.Height);
                        pickedG = System.Drawing.Graphics.FromImage(pickedImage);

                        fromR.X = (int)System.Math.Floor((initImage.Width - initImage.Height * templateRate) / 2);
                        fromR.Y = 0;
                        fromR.Width = (int)System.Math.Floor(initImage.Height * templateRate);
                        fromR.Height = initImage.Height;

                        toR.X = 0;
                        toR.Y = 0;
                        toR.Width = (int)System.Math.Floor(initImage.Height * templateRate);
                        toR.Height = initImage.Height;
                    }

                    //设置质量
                    SetGraphicsMode(pickedG);

                    //裁剪
                    pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);

                    //按模版大小生成最终图片
                    System.Drawing.Image templateImage = new System.Drawing.Bitmap(maxWidth, maxHeight);
                    System.Drawing.Graphics templateG = System.Drawing.Graphics.FromImage(templateImage);
                    //设置图片质量
                    SetGraphicsMode(templateG);

                    templateG.Clear(Color.White);
                    templateG.DrawImage(pickedImage, new System.Drawing.Rectangle(0, 0, maxWidth, maxHeight), new System.Drawing.Rectangle(0, 0, pickedImage.Width, pickedImage.Height), System.Drawing.GraphicsUnit.Pixel);

                    //保存缩略图
                    templateImage.Save(fileSavePath, ici, ep);

                    //templateImage.Save(fileSavePath, initImage.RawFormat);

                    //释放资源
                    templateG.Dispose();
                    templateImage.Dispose();

                    pickedG.Dispose();
                    pickedImage.Dispose();
                }
            }

            //释放资源
            initImage.Dispose();
        }
        /// <summary>
        /// 指定长宽裁剪
        /// 按模版比例最大范围的裁剪图片并缩放至模版尺寸
        /// </summary>
        /// <param name="fromFile">原图Stream对象</param>
        /// <param name="maxWidth">最大宽(单位:px)</param>
        /// <param name="maxHeight">最大高(单位:px)</param>
        /// <param name="quality">质量（范围0-100）</param>
        /// <returns>缩略图</returns>
        public static System.Drawing.Image CutForRectangle(System.IO.FileStream fromFile, int maxWidth, int maxHeight, int quality, out ImageCodecInfo ici, out EncoderParameters ep)
        {
            ici = null;
            ep = null;
            ImageEnoders(fromFile, quality, out ici, out ep);

            //从文件获取原始图片，并使用流中嵌入的颜色管理信息
            System.Drawing.Image initImage = System.Drawing.Image.FromStream(fromFile, true);

            //原图宽高均小于模版，不作处理，直接保存
            if (initImage.Width <= maxWidth && initImage.Height <= maxHeight)
            {
                return initImage;
            }
            else
            {
                //模版的宽高比例
                double templateRate = (double)maxWidth / maxHeight;
                //原图片的宽高比例
                double initRate = (double)initImage.Width / initImage.Height;

                //原图与模版比例相等，直接缩放
                if (templateRate == initRate)
                {
                    //按模版大小生成最终图片
                    System.Drawing.Image templateImage = new System.Drawing.Bitmap(maxWidth, maxHeight);
                    System.Drawing.Graphics templateG = System.Drawing.Graphics.FromImage(templateImage);
                    //设置图片质量
                    SetGraphicsMode(templateG);

                    templateG.Clear(Color.White);
                    templateG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, maxWidth, maxHeight), new System.Drawing.Rectangle(0, 0, initImage.Width, initImage.Height), System.Drawing.GraphicsUnit.Pixel);

                    return templateImage;
                }
                //原图与模版比例不等，裁剪后缩放
                else
                {
                    //裁剪对象
                    System.Drawing.Image pickedImage = null;
                    System.Drawing.Graphics pickedG = null;

                    //定位
                    Rectangle fromR = new Rectangle(0, 0, 0, 0);//原图裁剪定位
                    Rectangle toR = new Rectangle(0, 0, 0, 0);//目标定位

                    //宽为标准进行裁剪
                    if (templateRate > initRate)
                    {
                        //裁剪对象实例化
                        pickedImage = new System.Drawing.Bitmap(initImage.Width, (int)System.Math.Floor(initImage.Width / templateRate));
                        pickedG = System.Drawing.Graphics.FromImage(pickedImage);

                        //裁剪源定位
                        fromR.X = 0;
                        fromR.Y = (int)System.Math.Floor((initImage.Height - initImage.Width / templateRate) / 2);
                        fromR.Width = initImage.Width;
                        fromR.Height = (int)System.Math.Floor(initImage.Width / templateRate);

                        //裁剪目标定位
                        toR.X = 0;
                        toR.Y = 0;
                        toR.Width = initImage.Width;
                        toR.Height = (int)System.Math.Floor(initImage.Width / templateRate);
                    }
                    //高为标准进行裁剪
                    else
                    {
                        pickedImage = new System.Drawing.Bitmap((int)System.Math.Floor(initImage.Height * templateRate), initImage.Height);
                        pickedG = System.Drawing.Graphics.FromImage(pickedImage);

                        fromR.X = (int)System.Math.Floor((initImage.Width - initImage.Height * templateRate) / 2);
                        fromR.Y = 0;
                        fromR.Width = (int)System.Math.Floor(initImage.Height * templateRate);
                        fromR.Height = initImage.Height;

                        toR.X = 0;
                        toR.Y = 0;
                        toR.Width = (int)System.Math.Floor(initImage.Height * templateRate);
                        toR.Height = initImage.Height;
                    }

                    //设置质量
                    SetGraphicsMode(pickedG);

                    //裁剪
                    pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);

                    //按模版大小生成最终图片
                    System.Drawing.Image templateImage = new System.Drawing.Bitmap(maxWidth, maxHeight);
                    System.Drawing.Graphics templateG = System.Drawing.Graphics.FromImage(templateImage);
                    //设置图片质量
                    SetGraphicsMode(templateG);

                    templateG.Clear(Color.White);
                    templateG.DrawImage(pickedImage, new System.Drawing.Rectangle(0, 0, maxWidth, maxHeight), new System.Drawing.Rectangle(0, 0, pickedImage.Width, pickedImage.Height), System.Drawing.GraphicsUnit.Pixel);

                    //释放资源
                    templateG.Dispose();
                    pickedG.Dispose();
                    pickedImage.Dispose();
                    //保存缩略图
                    return templateImage;
                }
            }
        }
        #endregion

        #region 等比缩放
        /// <summary>
        /// 图片等比缩放
        /// </summary>
        /// <param name="initImage"></param>
        /// <param name="fileSavePath"></param>
        /// <param name="targetWidth"></param>
        /// <param name="targetHeight"></param>
        /// <param name="watermarkText"></param>
        /// <param name="watermarkImage"></param>
        /// <param name="quality"></param>
        /// <param name="ici"></param>
        /// <param name="ep"></param>
        public static void ZoomAuto(System.Drawing.Image initImage, string fileSavePath, System.Double targetWidth, System.Double targetHeight, string watermarkText, string watermarkImage, int quality, ImageCodecInfo ici, EncoderParameters ep)
        {
            //原图宽高均小于模版，不作处理，直接保存
            if (initImage.Width <= targetWidth && initImage.Height <= targetHeight)
            {
                //文字水印
                if (!string.IsNullOrEmpty(watermarkText))
                {
                    using (System.Drawing.Graphics gWater = System.Drawing.Graphics.FromImage(initImage))
                    {
                        System.Drawing.Font fontWater = new Font("黑体", 10);
                        System.Drawing.Brush brushWater = new SolidBrush(Color.White);
                        gWater.DrawString(watermarkText, fontWater, brushWater, 10, 10);
                        gWater.Dispose();
                    }
                }

                //透明图片水印
                if (!string.IsNullOrEmpty(watermarkImage))
                {
                    if (File.Exists(watermarkImage))
                    {
                        //获取水印图片
                        using (System.Drawing.Image wrImage = System.Drawing.Image.FromFile(watermarkImage))
                        {
                            //水印绘制条件：原始图片宽高均大于或等于水印图片
                            if (initImage.Width >= wrImage.Width && initImage.Height >= wrImage.Height)
                            {
                                Graphics gWater = Graphics.FromImage(initImage);

                                //透明属性
                                ImageAttributes imgAttributes = new ImageAttributes();
                                ColorMap colorMap = new ColorMap();
                                colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
                                colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
                                ColorMap[] remapTable = { colorMap };
                                imgAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

                                float[][] colorMatrixElements = { 
                                   new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  0.0f,  0.0f,  0.5f, 0.0f},//透明度:0.5
                                   new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                };

                                ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);
                                imgAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                gWater.DrawImage(wrImage, new Rectangle(initImage.Width - wrImage.Width, initImage.Height - wrImage.Height, wrImage.Width, wrImage.Height), 0, 0, wrImage.Width, wrImage.Height, GraphicsUnit.Pixel, imgAttributes);

                                gWater.Dispose();
                            }
                            wrImage.Dispose();
                        }
                    }
                }
                //保存
                initImage.Save(fileSavePath, ici, ep);

                //保存
                //initImage.Save(fileSavePath, initImage.RawFormat);
            }
            else
            {
                //缩略图宽、高计算
                double newWidth = initImage.Width;
                double newHeight = initImage.Height;

                //宽大于高或宽等于高（横图或正方）
                if (initImage.Width > initImage.Height || initImage.Width == initImage.Height)
                {
                    //如果宽大于模版
                    if (initImage.Width > targetWidth)
                    {
                        //宽按模版，高按比例缩放
                        newWidth = targetWidth;
                        newHeight = initImage.Height * (targetWidth / initImage.Width);
                    }
                }
                //高大于宽（竖图）
                else
                {
                    //如果高大于模版
                    if (initImage.Height > targetHeight)
                    {
                        //高按模版，宽按比例缩放
                        newHeight = targetHeight;
                        newWidth = initImage.Width * (targetHeight / initImage.Height);
                    }
                }

                //生成新图
                //新建一个bmp图片
                System.Drawing.Image newImage = new System.Drawing.Bitmap((int)newWidth, (int)newHeight);
                //新建一个画板
                System.Drawing.Graphics newG = System.Drawing.Graphics.FromImage(newImage);

                //设置质量
                SetGraphicsMode(newG);

                //置背景色
                newG.Clear(Color.White);
                //画图
                newG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, newImage.Width, newImage.Height), new System.Drawing.Rectangle(0, 0, initImage.Width, initImage.Height), System.Drawing.GraphicsUnit.Pixel);

                //文字水印
                if (!string.IsNullOrEmpty(watermarkText))
                {
                    using (System.Drawing.Graphics gWater = System.Drawing.Graphics.FromImage(newImage))
                    {
                        System.Drawing.Font fontWater = new Font("宋体", 10);
                        System.Drawing.Brush brushWater = new SolidBrush(Color.White);
                        gWater.DrawString(watermarkText, fontWater, brushWater, 10, 10);
                        gWater.Dispose();
                    }
                }

                //透明图片水印
                if (!string.IsNullOrEmpty(watermarkImage))
                {
                    if (File.Exists(watermarkImage))
                    {
                        //获取水印图片
                        using (System.Drawing.Image wrImage = System.Drawing.Image.FromFile(watermarkImage))
                        {
                            //水印绘制条件：原始图片宽高均大于或等于水印图片
                            if (newImage.Width >= wrImage.Width && newImage.Height >= wrImage.Height)
                            {
                                Graphics gWater = Graphics.FromImage(newImage);

                                //透明属性
                                ImageAttributes imgAttributes = new ImageAttributes();
                                ColorMap colorMap = new ColorMap();
                                colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
                                colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
                                ColorMap[] remapTable = { colorMap };
                                imgAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

                                float[][] colorMatrixElements = { 
                                   new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  0.0f,  0.0f,  0.5f, 0.0f},//透明度:0.5
                                   new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                };

                                ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);
                                imgAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                gWater.DrawImage(wrImage, new Rectangle(newImage.Width - wrImage.Width, newImage.Height - wrImage.Height, wrImage.Width, wrImage.Height), 0, 0, wrImage.Width, wrImage.Height, GraphicsUnit.Pixel, imgAttributes);
                                gWater.Dispose();
                            }
                            wrImage.Dispose();
                        }
                    }
                }

                //保存缩略图
                newImage.Save(fileSavePath, ici, ep);

                //保存缩略图
                //newImage.Save(fileSavePath, initImage.RawFormat);

                //释放资源
                newG.Dispose();
                newImage.Dispose();
            }
        }
        /// <summary>
        /// 图片等比缩放
        /// </summary>
        /// <param name="fromFile">原图Stream对象</param>
        /// <param name="fileSavePath">缩略图存放地址</param>
        /// <param name="targetWidth">指定的最大宽度</param>
        /// <param name="targetHeight">指定的最大高度</param>
        /// <param name="watermarkText">水印文字(为""表示不使用水印)</param>
        /// <param name="watermarkImage">水印图片路径(为""表示不使用水印)</param>
        /// <param name="quality">质量（范围0-100）</param>
        public static void ZoomAuto(System.IO.FileStream fromFile, string fileSavePath, System.Double targetWidth, System.Double targetHeight, string watermarkText, string watermarkImage, int quality)
        {
            string dir = IOHelper.GetDirectoryName(fileSavePath);
            IOHelper.CreateDirectory(dir);
            IOHelper.DeleteFile(fileSavePath);

            ImageCodecInfo ici;
            EncoderParameters ep;
            ImageEnoders(fromFile, quality, out ici, out ep);
            //原始图片（获取原始图片创建对象，并使用流中嵌入的颜色管理信息）
            using (System.Drawing.Image initImage = System.Drawing.Image.FromStream(fromFile, true))
            {
                ZoomAuto(initImage, fileSavePath, targetWidth, targetHeight, watermarkText, watermarkImage, quality, ici, ep);
            }
        }
        /// <summary>
        /// 图片等比缩放
        /// </summary>
        /// <param name="initImage"></param>
        /// <param name="targetWidth"></param>
        /// <param name="targetHeight"></param>
        /// <param name="watermarkText"></param>
        /// <param name="watermarkImage"></param>
        /// <param name="quality"></param>
        /// <param name="ici"></param>
        /// <param name="ep"></param>
        /// <returns></returns>
        public static Image ZoomAuto(System.Drawing.Image initImage, System.Double targetWidth, System.Double targetHeight, string watermarkText, string watermarkImage, int quality, ImageCodecInfo ici, EncoderParameters ep)
        {
            Image resultImage = null;

            //原图宽高均小于模版，不作处理，直接保存
            if (initImage.Width <= targetWidth && initImage.Height <= targetHeight)
            {
                bool isSource = true;
                //文字水印
                if (!string.IsNullOrEmpty(watermarkText))
                {
                    using (System.Drawing.Graphics gWater = System.Drawing.Graphics.FromImage(initImage))
                    {
                        System.Drawing.Font fontWater = new Font("黑体", 10);
                        System.Drawing.Brush brushWater = new SolidBrush(Color.White);
                        gWater.DrawString(watermarkText, fontWater, brushWater, 10, 10);
                        gWater.Dispose();
                    }
                    isSource = false;
                }

                //透明图片水印
                if (!string.IsNullOrEmpty(watermarkImage))
                {
                    if (File.Exists(watermarkImage))
                    {
                        //获取水印图片
                        using (System.Drawing.Image wrImage = System.Drawing.Image.FromFile(watermarkImage))
                        {
                            //水印绘制条件：原始图片宽高均大于或等于水印图片
                            if (initImage.Width >= wrImage.Width && initImage.Height >= wrImage.Height)
                            {
                                Graphics gWater = Graphics.FromImage(initImage);

                                //透明属性
                                ImageAttributes imgAttributes = new ImageAttributes();
                                ColorMap colorMap = new ColorMap();
                                colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
                                colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
                                ColorMap[] remapTable = { colorMap };
                                imgAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

                                float[][] colorMatrixElements = { 
                                   new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  0.0f,  0.0f,  0.5f, 0.0f},//透明度:0.5
                                   new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                };

                                ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);
                                imgAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                gWater.DrawImage(wrImage, new Rectangle(initImage.Width - wrImage.Width, initImage.Height - wrImage.Height, wrImage.Width, wrImage.Height), 0, 0, wrImage.Width, wrImage.Height, GraphicsUnit.Pixel, imgAttributes);

                                gWater.Dispose();
                            }
                            wrImage.Dispose();
                        }
                    }
                    isSource = false;
                }
                if (isSource)
                {
                    resultImage = (System.Drawing.Image)initImage.Clone();
                }
                else
                {
                    //保存
                    using (MemoryStream ms = new MemoryStream())
                    {
                        initImage.Save(ms, ici, ep);
                        resultImage = Image.FromStream(ms);
                    }
                }
            }
            else
            {
                //缩略图宽、高计算
                double newWidth = initImage.Width;
                double newHeight = initImage.Height;

                //宽大于高或宽等于高（横图或正方）
                if (initImage.Width > initImage.Height || initImage.Width == initImage.Height)
                {
                    //如果宽大于模版
                    if (initImage.Width > targetWidth)
                    {
                        //宽按模版，高按比例缩放
                        newWidth = targetWidth;
                        newHeight = initImage.Height * (targetWidth / initImage.Width);
                    }
                }
                //高大于宽（竖图）
                else
                {
                    //如果高大于模版
                    if (initImage.Height > targetHeight)
                    {
                        //高按模版，宽按比例缩放
                        newHeight = targetHeight;
                        newWidth = initImage.Width * (targetHeight / initImage.Height);
                    }
                }

                //生成新图
                //新建一个bmp图片
                System.Drawing.Image newImage = new System.Drawing.Bitmap((int)newWidth, (int)newHeight);
                //新建一个画板
                System.Drawing.Graphics newG = System.Drawing.Graphics.FromImage(newImage);

                //设置质量
                SetGraphicsMode(newG);

                //置背景色
                newG.Clear(Color.White);
                //画图
                newG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, newImage.Width, newImage.Height), new System.Drawing.Rectangle(0, 0, initImage.Width, initImage.Height), System.Drawing.GraphicsUnit.Pixel);

                //文字水印
                if (!string.IsNullOrEmpty(watermarkText))
                {
                    using (System.Drawing.Graphics gWater = System.Drawing.Graphics.FromImage(newImage))
                    {
                        System.Drawing.Font fontWater = new Font("宋体", 10);
                        System.Drawing.Brush brushWater = new SolidBrush(Color.White);
                        gWater.DrawString(watermarkText, fontWater, brushWater, 10, 10);
                        gWater.Dispose();
                    }
                }

                //透明图片水印
                if (!string.IsNullOrEmpty(watermarkImage))
                {
                    if (File.Exists(watermarkImage))
                    {
                        //获取水印图片
                        using (System.Drawing.Image wrImage = System.Drawing.Image.FromFile(watermarkImage))
                        {
                            //水印绘制条件：原始图片宽高均大于或等于水印图片
                            if (newImage.Width >= wrImage.Width && newImage.Height >= wrImage.Height)
                            {
                                Graphics gWater = Graphics.FromImage(newImage);

                                //透明属性
                                ImageAttributes imgAttributes = new ImageAttributes();
                                ColorMap colorMap = new ColorMap();
                                colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
                                colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
                                ColorMap[] remapTable = { colorMap };
                                imgAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

                                float[][] colorMatrixElements = { 
                                   new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  0.0f,  0.0f,  0.5f, 0.0f},//透明度:0.5
                                   new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                };

                                ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);
                                imgAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                gWater.DrawImage(wrImage, new Rectangle(newImage.Width - wrImage.Width, newImage.Height - wrImage.Height, wrImage.Width, wrImage.Height), 0, 0, wrImage.Width, wrImage.Height, GraphicsUnit.Pixel, imgAttributes);
                                gWater.Dispose();
                            }
                            wrImage.Dispose();
                        }
                    }
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    //保存缩略图
                    newImage.Save(ms, ici, ep);
                    resultImage = Image.FromStream(ms);
                }
                newImage.Dispose();

                //释放资源
                newG.Dispose();
            }

            return resultImage;
        }
        /// <summary>
        /// 图片等比缩放
        /// </summary>
        /// <param name="fromFile">原图Stream对象</param>
        /// <param name="targetWidth">指定的最大宽度</param>
        /// <param name="targetHeight">指定的最大高度</param>
        /// <param name="watermarkText">水印文字(为""表示不使用水印)</param>
        /// <param name="watermarkImage">水印图片路径(为""表示不使用水印)</param>
        /// <param name="quality">质量（范围0-100）</param>
        /// <returns>等比例缩放后的图片</returns>
        public static Image ZoomAuto(System.IO.FileStream fromFile, System.Double targetWidth, System.Double targetHeight, string watermarkText, string watermarkImage, int quality)
        {
            Image resultImage = null;
            ImageCodecInfo ici = null;
            EncoderParameters ep = null;
            ImageEnoders(fromFile, quality, out ici, out ep);
            //原始图片（获取原始图片创建对象，并使用流中嵌入的颜色管理信息）
            using (System.Drawing.Image initImage = System.Drawing.Image.FromStream(fromFile, true))
            {
                resultImage = ZoomAuto(initImage, targetWidth, targetHeight, watermarkText, watermarkImage, quality, ici, ep);
            }
            return resultImage;
        }
        #endregion

        #region 其它

        /// <summary>
        /// 设置图片质量
        /// </summary>
        /// <param name="g"></param>
        public static void SetGraphicsMode(System.Drawing.Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
        }
        /// <summary>
        /// 判断文件类型是否为WEB格式图片
        /// (注：JPG,GIF,BMP,PNG)
        /// </summary>
        /// <param name="contentType">HttpPostedFile.ContentType</param>
        /// <returns></returns>
        public static bool IsWebImage(string contentType)
        {
            if (contentType == "image/pjpeg" || contentType == "image/jpeg" || contentType == "image/gif" || contentType == "image/bmp" || contentType == "image/png")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 设置图片编码格式
        /// </summary>
        /// <param name="fromFile"></param>
        /// <param name="quality"></param>
        /// <param name="ici"></param>
        /// <param name="ep"></param>
        public static void ImageEnoders(System.IO.FileStream fromFile, int quality, out ImageCodecInfo ici, out EncoderParameters ep)
        {
            //关键质量控制
            //获取系统编码类型数组,包含了jpeg,bmp,png,gif,tiff
            ImageCodecInfo[] icis = ImageCodecInfo.GetImageEncoders();
            ici = null;
            //foreach (ImageCodecInfo i in icis)
            //{
            //    if (i.MimeType == "image/jpeg" || i.MimeType == "image/bmp" || i.MimeType == "image/png" || i.MimeType == "image/gif")
            //    {
            //        ici = i;
            //        break;
            //    }
            //}
            foreach (ImageCodecInfo i in icis)
            {
                if (i.FilenameExtension.ToLower().Contains(IOHelper.GetFileExtension(fromFile.Name).ToLower()))
                {
                    ici = i;
                    break;
                }
            }
            ep = new EncoderParameters(1);
            ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)quality);
        }

        #endregion

        /// <summary> 
        /// RLE 压缩解压算法 
        /// 将未压缩的数据分为不同块，每一个数据块由两个部分组成 
        ///   1：块头 2字节 
        ///    口口口口口口口口口口口口口口口口 
        ///    最高位标志位 ：   
        ///         0  不重复的一连串数据 
        ///         1  重复的一连串数据 
        ///   2：数据区域 
        ///  
        ///     当块头部分最高位是0时，后面的15位用来存放不重复数据的长度， 
        ///   因此可保存最多的不重复数据的长度为32767，数据区域就是一连串 
        ///   不重复的数据 
        ///     当块头部分最高位是1时，后面的15位用来存放重复的字节的长度， 
        ///   同样长度最大为32767，数据区域只有一个字节，即重复的那个数据。 
        ///   
        /// </summary> 
        public class CRLE
        {
            #region "成员"
            private const Int32 BLOCKMAX = 32767;       //块的最大长度 
            #endregion

            #region "压缩"
            /// <summary> 
            /// 压缩 
            /// 特殊情况下，压缩后的数据有可能比压缩前大 
            /// </summary> 
            /// <param name="p_Data">被压缩的数据</param> 
            /// <returns>压缩后的数据</returns> 
            public static Byte[] Compress(Byte[] p_Data)
            {
                Byte[] btRet = null;                       //压缩后的数据 
                Byte[] btTmp = null;                       //用来保存压缩数据的 
                Int32 iCmpressedLength = 0;                //压缩后的数据长度 
                Int32 i = 0;                               //读取原始数据循环用 
                Int32 j = 0;                               //写入压缩数据循环用 
                Int16 iContinue = 0;                       //连续相同的字节长度 
                Int16 iNContinue = 0;                      //不连续相同的字节长度 
                Boolean bContinueOld = false;              //前一个字节是否是连续相同的字节 

                Byte[] btBlockHead = new Byte[2];           //块头,2字节 
                Byte btChkOld;                              //前一个字节 
                Byte btChk;                                 //当前字节 

                //去处第一个数据 
                btChkOld = p_Data[0];
                iContinue = 1;
                iNContinue = 1;

                //从第二个数据开始循环 
                for (i = 1; i < p_Data.Length; i++)
                {
                    btChk = p_Data[i];//去处当前数据 

                    #region 与前一个相等-连续
                    if (btChk == btChkOld)
                    {
                        if (!bContinueOld && (iNContinue > 1))
                        {
                            /*前面一个数据是不连续相同的数据串并且不连续的长度大于一*/

                            //因为和前一个数据相同,所以和前一个数据将构成连续的相同的字节块, 
                            //而从上次压缩位置到前前一个数据为止的数据将要写入不连续的字节块。 

                            btTmp = null;
                            btTmp = new Byte[iCmpressedLength + 2 + iNContinue];

                            //保存原来数据 
                            if (iCmpressedLength != 0)
                            {
                                for (j = 0; j < iCmpressedLength; j++)
                                {
                                    btTmp[j] = btRet[j];
                                }
                            }

                            //设定块头 
                            iNContinue -= 1;          //这个地方要减去一，因为最后一个数据会划分到下一个连续的块中 
                            btBlockHead = null;
                            btBlockHead = System.BitConverter.GetBytes(iNContinue);
                            BitArray ba = new BitArray(btBlockHead);
                            ba.Set(15, false);                          //最高位设定为0 
                            ba.CopyTo(btBlockHead, 0);

                            btTmp[iCmpressedLength] = btBlockHead[0];
                            btTmp[iCmpressedLength + 1] = btBlockHead[1];

                            //写数据 
                            for (j = 0; j < iNContinue; j++)
                            {
                                btTmp[iCmpressedLength + 2 + j] = p_Data[i - 1 - iNContinue + j];
                            }

                            iCmpressedLength += 2 + iNContinue;    //压缩长度 

                            iContinue = 2;                         //前一个数据和当前数据将成为连续的字节块 
                            iNContinue = 1;
                            bContinueOld = true;

                            btRet = btTmp;
                        }
                        else if (bContinueOld && (iContinue == BLOCKMAX))
                        {
                            /*前一个数据是连续的字节块，并且长度达到了最大的字节块长度*/
                            //写入连续字节块数据 

                            btTmp = null;
                            btTmp = new Byte[iCmpressedLength + 3];
                            //保存原来数据 
                            if (iCmpressedLength != 0)
                            {
                                for (j = 0; j < iCmpressedLength; j++)
                                {
                                    btTmp[j] = btRet[j];
                                }
                            }

                            //设定头 
                            btBlockHead = null;
                            btBlockHead = System.BitConverter.GetBytes(iContinue);
                            BitArray ba = new BitArray(btBlockHead);
                            ba.Set(15, true);                          //最高位设定为1 
                            ba.CopyTo(btBlockHead, 0);

                            btTmp[iCmpressedLength] = btBlockHead[0];
                            btTmp[iCmpressedLength + 1] = btBlockHead[1];

                            //写数据 
                            btTmp[iCmpressedLength + 2] = p_Data[i - 1];

                            iCmpressedLength += 3;
                            btChkOld = p_Data[i];
                            iContinue = 1;
                            iNContinue = 1;

                            btRet = btTmp;
                        }
                        else
                        {
                            bContinueOld = true;
                            iContinue++;               //连续子节数自加 
                        }
                    }
                    #endregion
                    else
                    #region 与前一个不等-不连续
                    {
                        /*当前字节和前一个字节不同的逻辑处理*/

                        btChkOld = btChk;

                        if (!bContinueOld)
                        {
                            /** 
                             * 前一个字节是不连续的字节块 
                             * 两种处理逻辑： 
                             * 一种是不连续字节数达最大，则输出 
                             * 二种是不连续字节数++ 
                            */

                            if (iNContinue == BLOCKMAX)
                            {
                                /*写入*/

                                //不连续字节长度超过最大长度 
                                //写入不连续字节块 
                                btTmp = null;
                                btTmp = new Byte[iCmpressedLength + 2 + iNContinue];
                                //保存原来数据 
                                if (iCmpressedLength != 0)
                                {
                                    for (j = 0; j < iCmpressedLength; j++)
                                    {
                                        btTmp[j] = btRet[j];
                                    }
                                }

                                //设定块头 
                                btBlockHead = null;
                                btBlockHead = System.BitConverter.GetBytes(iNContinue);
                                BitArray ba = new BitArray(btBlockHead);
                                ba.Set(15, false);                          //最高位设定为0 
                                ba.CopyTo(btBlockHead, 0);

                                btTmp[iCmpressedLength] = btBlockHead[0];
                                btTmp[iCmpressedLength + 1] = btBlockHead[1];

                                //写数据 
                                for (j = 0; j < iNContinue; j++)
                                {
                                    btTmp[iCmpressedLength + 2 + j] = p_Data[i - 1 - iNContinue + j];
                                }

                                iCmpressedLength += 2 + iNContinue;
                                iContinue = 1;
                                iNContinue = 1;

                                btRet = btTmp;
                            }
                            else
                            {
                                /*不连续字结数++*/

                                bContinueOld = false;
                                iNContinue++;
                            }
                        }
                        else if (bContinueOld)
                        {
                            /*如果前一个字节连续，则写入前面连续的字节数据，并设定当前不连续*/
                            /* 
                             * 保存方法，把头数据写入btTmp第0/1个字节，连续的数的值写入2个字节 
                             */


                            bContinueOld = false;

                            //写入连续的字节块 

                            btTmp = null;
                            btTmp = new Byte[iCmpressedLength + 3];
                            //保存原来数据 
                            if (iCmpressedLength != 0)
                            {
                                for (j = 0; j < iCmpressedLength; j++)
                                {
                                    btTmp[j] = btRet[j];
                                }
                            }

                            //设定头 (连续高位为1) 
                            btBlockHead = null;
                            btBlockHead = System.BitConverter.GetBytes(iContinue);
                            BitArray ba = new BitArray(btBlockHead);
                            ba.Set(15, true);                          //最高位设定为1 
                            ba.CopyTo(btBlockHead, 0);

                            btTmp[iCmpressedLength] = btBlockHead[0];
                            btTmp[iCmpressedLength + 1] = btBlockHead[1];

                            //写数据 
                            btTmp[iCmpressedLength + 2] = p_Data[i - 1];

                            iCmpressedLength += 3;
                            //btChkOld = p_Data[i];//154已写 
                            iContinue = 1;
                            iNContinue = 1;

                            btRet = btTmp;
                        }
                    }
                    #endregion
                }

                #region 结束部分
                if (iContinue > 1)
                {
                    //最后为连续的字节块部分 
                    btTmp = null;
                    btTmp = new Byte[iCmpressedLength + 3];
                    //保存原来数据 
                    if (iCmpressedLength != 0)
                    {
                        for (j = 0; j < iCmpressedLength; j++)
                        {
                            btTmp[j] = btRet[j];
                        }
                    }

                    //设定头 
                    btBlockHead = null;
                    btBlockHead = System.BitConverter.GetBytes(iContinue);
                    BitArray ba = new BitArray(btBlockHead);
                    ba.Set(15, true);                          //最高位设定为1 
                    ba.CopyTo(btBlockHead, 0);

                    btTmp[iCmpressedLength] = btBlockHead[0];
                    btTmp[iCmpressedLength + 1] = btBlockHead[1];

                    //写数据 
                    btTmp[iCmpressedLength + 2] = p_Data[i - 1];

                    iCmpressedLength += 3;
                    btRet = btTmp;

                }
                else if (iNContinue >= 1)
                {
                    //最后为不连续的字节块部分 
                    //写入不连续字节块 
                    btTmp = null;
                    btTmp = new Byte[iCmpressedLength + 2 + iNContinue];
                    //保存原来数据 
                    if (iCmpressedLength != 0)
                    {
                        for (j = 0; j < iCmpressedLength; j++)
                        {
                            btTmp[j] = btRet[j];
                        }
                    }

                    //设定块头 
                    btBlockHead = null;
                    btBlockHead = System.BitConverter.GetBytes(iNContinue);
                    BitArray ba = new BitArray(btBlockHead);
                    ba.Set(15, false);                          //最高位设定为0 
                    ba.CopyTo(btBlockHead, 0);

                    btTmp[iCmpressedLength] = btBlockHead[0];
                    btTmp[iCmpressedLength + 1] = btBlockHead[1];

                    //写数据 
                    for (j = 0; j < iNContinue; j++)
                    {
                        btTmp[iCmpressedLength + 2 + j] = p_Data[i - iNContinue + j];
                    }

                    iCmpressedLength += 2 + iNContinue;
                    btRet = btTmp;
                }
                #endregion

                return btRet;
            }
            #endregion

            //压缩后的数据，用来测试解压 
            public static Byte[] test = new Byte[15]{ 
            5,128,8, //5个连续8 
            5,128,7, //5个连续7 
            5,128,6, 
            5,128,5, 
            5,128,4 
        };

            #region "解压"
            /// <summary> 
            /// 解压 
            /// </summary> 
            /// <param name="p_Data">需要解压得数据</param> 
            /// <returns>解压后的数据</returns> 
            public static Byte[] UnCompress(Byte[] p_Data)
            {
                Byte[] btRet = null;           //结果 
                Byte[] btTmp = null;           //临时用来保存解压数据 

                Int32 i = 0;
                Int32 j = 0;
                Byte[] btHead = new Byte[2];   //块头 
                Int16 iDataCount = 0;          //数据块的长度 
                BitArray ba = null;            //比特数组 
                Int32 iDataLen = 0;            //解压后数据长度 

                for (i = 0; i < p_Data.Length; i++)
                {
                    //读出块头 
                    btHead[0] = p_Data[i];
                    btHead[1] = p_Data[i + 1];

                    ba = new BitArray(btHead);

                    //计算出块的长度 转换成无符号整数，所以高位即使为1，也忽略 
                    iDataCount = (Int16)BitConverter.ToUInt16(btHead, 0);

                    if (ba.Get(15))
                    {
                        //最高位是1，为连续的字节块 
                        iDataCount = (Int16)((Int32)iDataCount + 32768); //最高位为1，算出的长度其实为负值 

                        //解压数据 
                        btTmp = null;
                        btTmp = new Byte[iDataLen + iDataCount];

                        //保存数据 
                        if (iDataLen > 0)
                        {
                            for (j = 0; j < iDataLen; j++)
                            {
                                btTmp[j] = btRet[j];
                            }
                        }

                        //解压数据 
                        for (j = 0; j < iDataCount; j++)
                        {
                            btTmp[iDataLen + j] = p_Data[i + 2];
                        }

                        i += 2;
                    }
                    else
                    {
                        //不连续的字节块 
                        btTmp = null;
                        btTmp = new Byte[iDataLen + iDataCount];
                        //保存数据 
                        if (iDataLen > 0)
                        {
                            for (j = 0; j < iDataLen; j++)
                            {
                                btTmp[j] = btRet[j];
                            }
                        }

                        //解压数据 
                        for (j = 0; j < iDataCount; j++)
                        {
                            btTmp[iDataLen + j] = p_Data[i + 2 + j];
                        }

                        i += 1 + iDataCount;
                    }

                    iDataLen += iDataCount;
                    btRet = btTmp;
                }

                return btRet;
            }
            #endregion

        }
    }
}
