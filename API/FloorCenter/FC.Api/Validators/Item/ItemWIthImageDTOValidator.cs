using FC.Api.DTOs.Item;
using FC.Api.Helpers;
using FC.Api.Validators.Size;
using FluentValidation;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Validators.Item
{
    public class ItemWIthImageDTOValidator : AbstractValidator<ItemWithImageDTO>
    {
        private readonly DataContext _context;
        private readonly AppSettings _appSettings;

        public ItemWIthImageDTOValidator(DataContext context, IOptions<AppSettings> appSettings, bool runDefaultValidations = true)
        {
            this._context = context;
            _appSettings = appSettings.Value;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            if (runDefaultValidations)
            {
                //  Validate Id
                RuleFor(p => p.Id)
                    .Must(RecordExist)
                        .When(p => p.Id != 0)
                        .WithMessage("Record is not valid");


                //  Validate Code
                RuleFor(p => p.Code)
                    .NotEmpty()
                    .MaximumLength(50);


                //  Validate Serial Number
                RuleFor(x => x)
                    .NotEmpty()
                    .Must(SerialNumberBeNineDigit)
                        .WithMessage("Serial Number must be 9 digits")
                        .WithName("SerialNumber")
                    .Must(SerialNumberStillAvailable)
                        .WithMessage("Serial Number is already registered")
                        .WithName("SerialNumber");


                //  Validate Name
                RuleFor(p => p.Name)
                    .NotEmpty()
                    .MaximumLength(255);


                //  Validate SRP
                RuleFor(p => p.SRP)
                    .NotEmpty()
                    .GreaterThan(0)
                    .Must(srpLengthValid)
                    .WithMessage("The length of 'SRP' must be 16 characters or fewer.");


                RuleFor(p => p.Cost)
                    .NotEmpty()
                    .GreaterThan(0)
                    .Must(srpLengthValid)
                    .WithMessage("The length of 'Cost' must be 16 characters or fewer.");

                //  Validate Size ID
                RuleFor(p => p.SizeId)
                    .NotEmpty()
                    .Must(new SizeDTOValidator(context, false).IdValid)
                        .WithMessage("{PropertyName} is not valid");

                //  Validate Image Name
                RuleFor(p => p)
                    .Must(CheckImageIfExist)
                        .WithMessage("Image Name do not exist")
                        .WithName("ImageName");

                //  Validate Tonality
                RuleFor(p => p.Tonality)
                    .NotEmpty()
                        .MaximumLength(50);


                //  Validate Code & Tonality
                RuleFor(x => x)
                    .Must(CodeAndTonalityStillAvailable)
                        .WithMessage("Item is already registered");

            }

        }

        private bool RecordExist(int id)
        {
            var count = this._context.Items.Where(x => x.Id == id).Count();
            if (count != 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool CodeAndTonalityStillAvailable(ItemDTO model)
        {
            if (!string.IsNullOrEmpty(model.Code) && !string.IsNullOrEmpty(model.Code))
            {
                var obj = this._context.Items.Where(x => x.Code.ToLower() == model.Code.ToLower() && x.Tonality.ToLower() == model.Tonality.ToLower() && x.Id != model.Id).SingleOrDefault();

                if (obj == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }


        private bool SerialNumberStillAvailable(ItemDTO model)
        {

            if (!string.IsNullOrEmpty(model.SerialNumber))
            {
                var obj = this._context.Items.Where(x => x.SerialNumber == model.SerialNumber && x.Id != model.Id).Count();

                if (obj >= 1)
                {
                    return false;
                }
            }
            return true;

        }

        private bool SerialNumberBeNineDigit(ItemDTO model)
        {

            if (!string.IsNullOrEmpty(model.SerialNumber))
            {
                if (model.SerialNumber.ToString().Length != 9)
                {
                    return false;
                }
            }
            return true;

        }


        /// <summary>
        /// This function is used to validate if selected Item ID is registered.
        /// </summary>
        /// <param name="id">ID to be checked if registered.</param>
        /// <returns>False if not valid means not registered, otherwise True</returns>
        public bool IdValid(int? id)
        {
            if (id.HasValue)
            {
                var count = this._context.Items.Where(x => x.Id == id).Count();
                if (count != 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }

            return true;
        }

        public bool srpLengthValid(decimal? srp)
        {
            if(srp.HasValue)
            {
                 if(srp.ToString().Length > 16)
                {
                    return false;
                }
                
            }
            return true;
        }

        private bool CheckImageIfExist(ItemWithImageDTO model)
        {
            if (model.Id == 0)
            {
                if (!string.IsNullOrEmpty(model.ImageName))
                {
                    return File.Exists(Path.Combine(_appSettings.Item_temp_image, model.ImageName));
                }
                return true;
            }
            else
            {
                var count = this._context.Items.Where(x => x.Id == model.Id && x.ImageName == model.ImageName).Count();

                if (count == 1) 
                {
                    // During Update : Image is not changed.
                    if (!string.IsNullOrEmpty(model.ImageName))
                    {
                        return File.Exists(Path.Combine(_appSettings.Item_image, model.ImageName));
                    }
                    return true;
                }
                else
                {
                    // During Update : Image is changed
                    if (!string.IsNullOrEmpty(model.ImageName))
                    {
                        return File.Exists(Path.Combine(_appSettings.Item_temp_image, model.ImageName));
                    }
                    return true;
                }
            }
        }


    }
}
