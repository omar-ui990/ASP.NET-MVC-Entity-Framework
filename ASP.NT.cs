[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
{
    if (ModelState.IsValid)
    {
        // Start with a base of $50 / month
        decimal monthlyQuote = 50;

        // Age logic
        int age = DateTime.Now.Year - insuree.DateOfBirth.Year;
        if (insuree.DateOfBirth > DateTime.Now.AddYears(-age)) age--; // Precision check

        if (age <= 18) monthlyQuote += 100;
        else if (age >= 19 && age <= 25) monthlyQuote += 50;
        else monthlyQuote += 25;

        // Car Year logic
        if (insuree.CarYear < 2000) monthlyQuote += 25;
        if (insuree.CarYear > 2015) monthlyQuote += 25;

        // Car Make/Model logic
        if (insuree.CarMake.ToLower() == "porsche")
        {
            monthlyQuote += 25;
            if (insuree.CarModel.ToLower() == "911 carrera")
            {
                monthlyQuote += 25;
            }
        }

        // Speeding Tickets ($10 per ticket)
        monthlyQuote += (insuree.SpeedingTickets * 10);

        // DUI logic (Add 25%)
        if (insuree.DUI)
        {
            monthlyQuote = monthlyQuote * 1.25m;
        }

        // Coverage logic (Add 50%)
        if (insuree.CoverageType) // Assuming bool: true = full coverage
        {
            monthlyQuote = monthlyQuote * 1.50m;
        }

        // Assign the final calculation to the object
        insuree.Quote = monthlyQuote;

        db.Insurees.Add(insuree);
        db.SaveChanges();
        return RedirectToAction("Index");
    }

    return View(insuree);
}
@model IEnumerable<CarInsurance.Models.Insuree>

<h2>Admin Dashboard: All Quotes</h2>

<table class="table">
    <tr>
        <th>First Name</th>
        <th>Last Name</th>
        <th>Email Address</th>
        <th>Quote Amount</th>
    </tr>

@foreach (var item in Model) {
    <tr>
        <td>@Html.DisplayFor(modelItem => item.FirstName)</td>
        <td>@Html.DisplayFor(modelItem => item.LastName)</td>
        <td>@Html.DisplayFor(modelItem => item.EmailAddress)</td>
        <td>@String.Format("{0:C}", item.Quote)</td>
    </tr>
}
</table>
