﻿using Core.GroupByModels;
using Core.Models;
using DAL.Persistence;
using Newtonsoft.Json;
using RealCache.Persistence.Migrations;
using System.Collections.ObjectModel;
using System.Windows.Input;
using VaccineApp.ViewModels.Base;
using VaccineApp.Views.Mobilizer.Home.Status;

namespace VaccineApp.ViewModels.Mobilizer.Home.Status;

public class StatusViewModel : ViewModelBase
{
    private readonly UnitOfWork _unitOfWork;
    private readonly DbContext<PeriodModel> _dbContext;
    private ObservableCollection<ChildrenGroupByHouseNoModel> _childrenGroupByFamily;
    private ChildModel _selectedChild;

    public StatusViewModel(UnitOfWork unitOfWork, DbContext<PeriodModel> dbContext)
    {
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
        ChildrenGroupByFamily = new();
        SelectedChild = new();
        GetPeriod();
        ChildDetailsCommand = new Command(ChildDetails);
    }

    private async void GetPeriod()
    {
        try
        {
            var s = await _unitOfWork.GetActivePeriod();
            _dbContext.CreateDB("mobilizer", "user");
            _dbContext.CreateTable();
            _dbContext.Insert(s);
        }
        catch (Exception)
        {
            return;
        }
    }

    private async void ChildDetails()
    {
        if (SelectedChild == null)
        {
            return;
        }
        else
        {
            var SelectedItemJson = JsonConvert.SerializeObject(SelectedChild);
            var route = $"{nameof(ChildDetailsPage)}?Child={SelectedItemJson}";
            await Shell.Current.GoToAsync(route);
            SelectedChild = null;
        }
    }

    public async void Get()
    {
        try
        {
            var f = await _unitOfWork.GetFamilies();

            foreach (var item in f)
            {
                var c = await _unitOfWork.GetChilds(item.Id.ToString());

                ChildrenGroupByFamily.Add(new ChildrenGroupByHouseNoModel(item.HouseNo, c.ToList()));
            }
        }
        catch (Exception)
        {
            return;
        }
    }

    public ChildModel SelectedChild
    {
        get { return _selectedChild; }
        set { _selectedChild = value; OnPropertyChanged(); }
    }
    public ObservableCollection<ChildrenGroupByHouseNoModel> ChildrenGroupByFamily
    {
        get { return _childrenGroupByFamily; }
        set { _childrenGroupByFamily = value; OnPropertyChanged(); }
    }
    public ICommand ChildDetailsCommand { private set; get; }
    public void Clear()
    {
        ChildrenGroupByFamily.Clear();
        SelectedChild = null;
    }
}
