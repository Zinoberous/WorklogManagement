﻿using WorklogManagement.Data;

namespace WorklogManagement.UI.ViewModels;

public class TicketsViewModel(IWorklogManagementService service) : BaseViewModel
{
    private readonly IWorklogManagementService _service = service;
}