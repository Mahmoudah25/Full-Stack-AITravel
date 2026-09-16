function popularActivityDropdown(){
    const stored = localStorage.getItem('lastRatedActivities'); 
    const select = document.getElementById('activityId');
    const submitBtn = document.getElementById('submitRatingBtn');
    const resDiv = document.getElementById('ratingResult');

    const activities = stored ? JSON.parse(stored) : [];

    if(!activities || activities.length === 0){
        select.innerHTML = '<option value="">No activities found</option>';
        select.disabled = true;
        submitBtn.disabled = true;
        resDiv.className = 'error';
        resDiv.innerHTML = 'Complete Trip First to rate an activity. <a href="index.html">Plan a Trip &rarr;</a>';
        return;
    }

    select.disabled = false;
    submitBtn.disabled = false;
    resDiv.className = '';
    resDiv.textContent = '';

    select.innerHTML = '<option value="">   -- Select an activity --   </option>';
    activities.forEach(activity => {
        const option = document.createElement('option');
        option.value = activity.Id;
        option.textContent = activity.name;
        select.appendChild(option);
    });
}
popularActivityDropdown();  
document.getElementById('ratingForm').addEventListener('submit',async function (e) {
    e.preventDefault();
    const submitbtn =  document.getElementById('submitRatingBtn');
    const resDiv = document.getElementById('ratingResult');

    if(!document.getElementById('activityId').value){
        resDiv.className = 'error';
        resDiv.textContent = 'Complete Trip First';
        return;
    }

    const ratingData = {
        activityId:document.getElementById('activityId').value,
        customerEmail:document.getElementById('customerEmail').value,
        score:document.getElementById('score').value,
        comment:document.getElementById('comment').value || null
    }

    submitbtn.disabled  =true;
    submitbtn.textContent ="Submiting...";
    resDiv.className = '';
    resDiv.textContent ='';
    try{
        const ratingId = await CreateRaing(ratingData);
        resDiv.className = 'success';
        resDiv.textContent = 'Thanks For Rating.'
        document.getElementById('ratingForm').reset();

    }
    catch(error){
        resDiv.className = 'error';
        resDiv.textContent = error.message || 'Failed to submit rating.';
        console.log(error);

    }finally{
        submitbtn.disabled  =false;
        submitbtn.textContent ="Submit";

    }
})