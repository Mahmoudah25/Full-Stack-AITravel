document.getElementById('tripForm').addEventListener('submit', async function (e) {
    e.preventDefault();

    const submitBtn = document.getElementById('submitBtn');
    const resultDiv = document.getElementById('result');

    const tripData = {
        destination: document.getElementById('destination').value,
        startDate: new Date(document.getElementById('startDate').value).toISOString(),
        days: parseInt(document.getElementById('days').value),
        budgetAmount: parseFloat(document.getElementById('budgetAmount').value),
        budgetCurrency: document.getElementById('budgetCurrency').value
    };

    submitBtn.disabled = true;
    submitBtn.textContent = 'Creating...';
    resultDiv.className = '';
    resultDiv.textContent = '';

    try {
        const tripId = await createTrip(tripData);
        resultDiv.className = 'success';
        resultDiv.textContent = 'Trip created! Redirecting...';

        const params = new URLSearchParams({
            destination: tripData.destination,
            days: tripData.days,
            budget: tripData.budgetAmount,
            currency:tripData.budgetCurrency
        });

        setTimeout(() => {
            window.location.href = `trip-details.html?tripId=${tripId}&${params.toString()}`;
        }, 800);

    } catch (error) {
        resultDiv.className = 'error';
        resultDiv.textContent = 'Failed to create trip. Please try again.';
        console.error(error);
        submitBtn.disabled = false;
        submitBtn.textContent = 'Create Trip';
    }
});