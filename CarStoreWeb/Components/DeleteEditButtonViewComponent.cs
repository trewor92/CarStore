using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace CarStoreWeb.Components
{
    public class DeleteEditButtonViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(bool IsAuthorized)
        {
            if (IsAuthorized)
                return View();
            else
                return await Task.FromResult<IViewComponentResult>(Content(string.Empty));
        }
    }
}
