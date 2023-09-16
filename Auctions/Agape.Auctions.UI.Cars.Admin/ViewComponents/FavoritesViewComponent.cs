﻿using Agape.Auctions.UI.Cars.Admin.Models;
using AgapeAPI.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using AgapeModel = Agape.Auctions.Models.Cars;
using AgapeModelImage = Agape.Auctions.Models.Images;
using Agape.Auctions.UI.Cars.Admin.Utilities;
using Microsoft.Extensions.Logging;

namespace Agape.Auctions.UI.Cars.Admin.ViewComponents
{
    public class FavoritesViewComponent : ViewComponent
    {
        private readonly IConfiguration configure;
        private readonly string apiBaseUrlCar;
        private readonly string apiBaseUrlCarImage;
        private readonly string defaultCarImageUrl;
        private readonly string closedStatus = "Closed";
        private readonly ILogger<FavoritesViewComponent> _logger;
        private LogHelperComponent logHelper;

        public FavoritesViewComponent(IConfiguration configuration, ILogger<FavoritesViewComponent> logger)
        {
            configure = configuration;
            _logger = logger;
            apiBaseUrlCar = configure.GetValue<string>("WebAPIBaseUrlCar");
            apiBaseUrlCarImage = configure.GetValue<string>("WebAPIBaseUrlCarImage");
            defaultCarImageUrl = configure.GetValue<string>("DefaultCarImageUrl");
            logHelper = new LogHelperComponent(configure, _logger);
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var lstCars = new List<AgapeModel.Car>();
            try
            {
                var response = await GetAllCars();
                if (string.IsNullOrEmpty(response.Item2))
                {
                    lstCars = response.Item1.Where(i => i.Status != closedStatus).ToList();
                    var imageResponse = await GetAllCarImages();

                    if (string.IsNullOrEmpty(imageResponse.Item2))
                    {
                        if (lstCars != null && lstCars.Any())
                        {
                            foreach (var car in lstCars)
                            {
                                var carImage = imageResponse.Item1.Where(i => i.Owner == car.Id);
                                if (carImage != null && carImage.Any())
                                {
                                    car.Thumbnail = (carImage.Where(i => i.Order == 1) != null && carImage.Where(i => i.Order == 1).Any()) ? carImage.Where(i => i.Order == 1).Select(i => i.Url).FirstOrDefault().ToString() : carImage.FirstOrDefault().Url;
                                }
                                else
                                {
                                    car.Thumbnail = defaultCarImageUrl;
                                }

                                if (car.Video == null)
                                    car.Video = new AgapeModel.Video();
                            }
                        }
                    }
                    else
                    {
                        logHelper.LogError(imageResponse.Item2);
                        ViewBag.CarError = imageResponse.Item2;
                    }
                }
                else
                {
                    logHelper.LogError(response.Item2);
                    ViewBag.CarError = response.Item2;
                }
            }
            catch(Exception ex)
            {
                logHelper.LogError(ex.ToString());
            }

            return View("Default", lstCars);
        }

        public async Task<(List<AgapeModel.Car>,string)> GetAllCars()
        {
            var lstCars = new List<AgapeModel.Car>();
            string error = string.Empty;
            try
            {
                using (var client = new HttpClient(new CustomHttpClientHandler(configure)))
                {
                    string endpoint = apiBaseUrlCar;
                    using (var Response = await client.GetAsync(endpoint))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            lstCars = await Response.Content.ReadAsAsync<List<AgapeModel.Car>>();
                        }
                        else
                        {
                            error = "Error from Car Service";
                            logHelper.LogError(Response.ReasonPhrase + " " + error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = "Exception while get all cars";
                logHelper.LogError(ex.ToString());
            }
            return (lstCars, error);
        }

        public async Task<(List<AgapeModelImage.Image>,string)> GetAllCarImages()
        {
            var carImages = new List<AgapeModelImage.Image>();
            string error = string.Empty;
            try
            {
                using (var client = new HttpClient(new CustomHttpClientHandler(configure)))
                {
                    string endpoint = apiBaseUrlCarImage;
                    using (var Response = await client.GetAsync(endpoint))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            carImages = await Response.Content.ReadAsAsync<List<AgapeModelImage.Image>>();
                        }
                        else
                        {
                            error = "Error from CarImage Service";
                            logHelper.LogError(Response.ReasonPhrase + " " + error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = "Exception while get all car images";
                logHelper.LogError(ex.ToString());
            }
            return (carImages, error);
        }
    }
}
