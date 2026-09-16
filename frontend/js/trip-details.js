const urlParams = new URLSearchParams(window.location.search);
initializeNotifications('test@example.com');
const currentTrip = {
    tripId: urlParams.get('tripId'),
    destination: urlParams.get('destination'),
    days: parseInt(urlParams.get('days')),
    budget: parseFloat(urlParams.get('budget')),
    currency: urlParams.get('currency') || 'EGP'
};

function formatCost(amount) {
    const symbols = {
        USD: '$',
        EGP: 'E£',
        EUR: '€',
        GBP: '£',
        SAR: 'SAR ',
        AED: 'AED ',
        KWD: 'KWD '
    };
    const symbol = symbols[currentTrip.currency] || `${currentTrip.currency} `;
    return `${symbol}${amount}`;
}

document.getElementById('tripTitle').textContent = `${currentTrip.destination} Trip`;

// ============================================
// Track activity payment state for this trip, so buttons stay correct
// even after a page reload. Two states:
//   - "pending": payment tab was opened, but not confirmed yet
//   - "paid":    backend confirmed the payment (via SignalR)
// ============================================
const PENDING_KEY = `pendingActivities_${currentTrip.tripId}`;
const PAID_KEY = `paidActivities_${currentTrip.tripId}`;
// Maps a bookingId -> activityId so we can update the right button
// when a "BookingStatusChanged" notification arrives.
const bookingIdToActivityId = new Map();

function getSet(key) {
    const stored = localStorage.getItem(key);
    return stored ? new Set(JSON.parse(stored)) : new Set();
}
function addToSet(key, value) {
    const set = getSet(key);
    set.add(value);
    localStorage.setItem(key, JSON.stringify([...set]));
}
function removeFromSet(key, value) {
    const set = getSet(key);
    set.delete(value);
    localStorage.setItem(key, JSON.stringify([...set]));
}

function markActivityPending(activityId) {
    addToSet(PENDING_KEY, activityId);
}
function markActivityPaid(activityId) {
    removeFromSet(PENDING_KEY, activityId);
    addToSet(PAID_KEY, activityId);
}

// Called by notifications.js whenever the backend confirms a booking's
// status changed (e.g. after the Paymob webhook fires).
window.handleBookingStatusChanged = function (data) {
    const activityId = bookingIdToActivityId.get(data.bookingId) || data.activityId;
    if (!activityId) return;

    const status = (data.newStatus || '').toLowerCase();
    const btn = document.querySelector(`.book-btn[data-activity-id="${activityId}"]`);

    if (status === 'paid' || status === 'confirmed' || status === 'completed') {
        markActivityPaid(activityId);
        if (btn) {
            btn.classList.remove('pending');
            btn.classList.add('paid');
            btn.textContent = '✓ Paid';
            btn.disabled = true;
        }
    } else if (status === 'failed' || status === 'cancelled' || status === 'canceled') {
        removeFromSet(PENDING_KEY, activityId);
        if (btn) {
            btn.classList.remove('pending', 'paid');
            btn.disabled = false;
            btn.textContent = 'Book & Pay';
        }
    }
};

// Fix stuck "Redirecting to payment..." / "Booking..." buttons when the page
// is restored from the browser's back-forward cache (e.g. user hits Back
// after being sent to the Paymob payment page).
window.addEventListener('pageshow', function (event) {
    if (event.persisted) {
        document.querySelectorAll('.book-btn').forEach(btn => {
            if (btn.disabled && !btn.classList.contains('paid') && !btn.classList.contains('pending')) {
                btn.disabled = false;
                btn.textContent = 'Book & Pay';
            }
        });
    }
});

document.getElementById('generateBtn').addEventListener('click', async function () {
    const generateBtn = document.getElementById('generateBtn');
    if (generateBtn.disabled) return
    const itineraryResultDiv = document.getElementById('itineraryResult');

    generateBtn.disabled = true;
    generateBtn.textContent = 'Generating...';
    itineraryResultDiv.innerHTML = '';

    try {
        const itinerary = await generateItinerary(currentTrip);
        SaveActivityForRating(itinerary);
        renderItinerary(itinerary);
    } catch (error) {
        // The AI itinerary generation call failed (API down, timeout, etc.) —
        // show a simple error message instead of leaving the section blank.
        itineraryResultDiv.innerHTML = '<p class="error">Failed to generate itinerary.</p>';
        console.error(error);
    } finally {
        generateBtn.disabled = false;
        generateBtn.textContent = 'Generate Itinerary';
    }
});

function renderItinerary(itinerary) {
    const container = document.getElementById('itineraryResult');
    container.innerHTML = '';

    itinerary.days.forEach(day => {
        const dayDiv = document.createElement('div');
        dayDiv.className = 'day-card';

        const dayTitle = document.createElement('h3');
        const weatherInfo = day.weatherCondition
            ? ` (${getWeatherIcon(day.weatherCondition)} ${Math.round(day.temperatureCelsius)}°C)`
            : '';
        dayTitle.textContent = `Day ${day.dayNumber}${weatherInfo}`;
        dayDiv.appendChild(dayTitle);

        const activityList = document.createElement('div');
        activityList.className = 'activity-list';

        day.activities.forEach(activity => {
            const activityDiv = document.createElement('div');
            activityDiv.className = 'activity-item';

            const icon = getActivityIcon(activity.type);
            const isBookable = activity.estimatedCost > 0;
            const isPaid = getSet(PAID_KEY).has(activity.activityId);
            const isPending = getSet(PENDING_KEY).has(activity.activityId);
            const mapQuery = encodeURIComponent(`${activity.placeName}, ${currentTrip.destination}`);
            const mapUrl = `https://www.google.com/maps/search/?api=1&query=${mapQuery}`;

            let actionHtml;
            if (!isBookable) {
                actionHtml = `<span class="free-label">Free</span>`;
            } else if (isPaid) {
                actionHtml = `<button class="book-btn paid" disabled>✓ Paid</button>`;
            } else if (isPending) {
                actionHtml = `<button class="book-btn pending" disabled>Payment pending...</button>`;
            } else {
                actionHtml = `<button class="book-btn" data-activity-id="${activity.activityId}" data-cost="${activity.estimatedCost}">Book & Pay</button>`;
            }

            activityDiv.innerHTML = `
                <div class="activity-time">${activity.time}</div>
                <div class="activity-details">
                    <span class="activity-icon">${icon}</span>
                    <span class="activity-name">${activity.placeName}</span>
                    <span class="activity-type">${activity.type}</span>
                    <a href="${mapUrl}" target="_blank" class="map-link">View on Map</a>
                </div>
                <div class="activity-cost">${formatCost(activity.estimatedCost)}</div>
                ${actionHtml}
            `;

            activityList.appendChild(activityDiv);
        });

        dayDiv.appendChild(activityList);
        container.appendChild(dayDiv);
    });

    const totalDiv = document.createElement('div');
    totalDiv.className = 'total-cost-card';
    totalDiv.innerHTML = `<span>Total Estimated Cost</span><strong>${formatCost(itinerary.totalEstimatedCost)}</strong>`;
    container.appendChild(totalDiv);

    attachBookingHandlers();
}

function SaveActivityForRating(itinerary){
    const activityMap = new Map(); // To avoid duplicates based on activityId
    itinerary.days.forEach(day => {
        day.activities.forEach(activity => {
            if(activity.activityId){
                activityMap.set(activity.activityId, {        
                    Id: activity.activityId,
                    name: activity.placeName,
                });
            }
        });
    });
    localStorage.setItem('lastRatedActivities', JSON.stringify([...activityMap.values()]));
}

function getActivityIcon(type) {
    const icons = {
        'attraction': '🏛️',
        'food': '🍽️',
        'meal': '🍽️',
        'entertainment': '🎭',
        'market': '🛍️',
        'shopping': '🛍️',
        'park': '🌳',
        'transfer': '🚗',
        'tour': '🗺️',
        'hotel': '🏨'
    };
    return icons[type.toLowerCase()] || '📍';
}
function getWeatherIcon(condition) 
{
    const icons = 
    {
        'clear': '☀️',
        'clouds': '☁️',
        'rain': '🌧️',
        'drizzle': '🌦️',
        'thunderstorm': '⛈️',
        'snow': '❄️',
        'mist': '🌫️',
    };
    return icons[condition.toLowerCase()] || '🌡️';
}

function attachBookingHandlers() {
    document.querySelectorAll('.book-btn').forEach(btn => {
        btn.addEventListener('click', async function () {
            const activityId = this.dataset.activityId;
            const cost = parseFloat(this.dataset.cost);

            // Open the tab immediately, synchronously, while we still have the
            // user's click gesture — browsers block window.open() called after
            // an await because it no longer counts as user-initiated.
            const paymentTab = window.open('', '_blank');
            if (paymentTab) {
                paymentTab.document.write('Preparing your payment, please wait...');
            }

            this.disabled = true;
            this.textContent = 'Booking...';

            try {
                const bookingId = await createBooking({
                    tripId: currentTrip.tripId,
                    activityId: activityId,
                    amount: cost,
                    currency: currentTrip.currency,
                    customerEmail: 'test@example.com'
                });

                bookingIdToActivityId.set(bookingId, activityId);

                this.textContent = 'Opening payment...';
                const paymentUrl = await initiatePayment(bookingId);

                if (paymentTab && !paymentTab.closed) {
                    paymentTab.location.href = paymentUrl;
                } else {
                    // Popup was blocked or closed by the user — fall back to same-tab redirect
                    window.location.href = paymentUrl;
                    return;
                }

                markActivityPending(activityId);
                this.classList.add('pending');
                this.textContent = 'Payment pending...';
                this.disabled = true;

            } catch (error) {
                // Either createBooking() or initiatePayment() failed — close the
                // blank tab we opened earlier (no point leaving it hanging),
                // reset the button so the user can try again, and let them know.
                if (paymentTab && !paymentTab.closed) paymentTab.close();
                this.disabled = false;
                this.textContent = 'Book & Pay';
                alert('Booking failed. Please try again.');
                console.error(error);
            }
        });
    });
}