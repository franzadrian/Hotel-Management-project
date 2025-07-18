// Room Filter Functionality
document.querySelectorAll('.v5-filter-option').forEach(option => {
    option.addEventListener('click', function() {
        // Update active state
        document.querySelectorAll('.v5-filter-option').forEach(opt => {
            opt.classList.remove('active');
        });
        this.classList.add('active');
        
        // Filter reviews
        const roomType = this.getAttribute('data-room');
        const reviewCards = document.querySelectorAll('.v5-review-card');
        
        reviewCards.forEach(card => {
            if (roomType === 'all' || card.getAttribute('data-room') === roomType) {
                card.style.display = 'block';
            } else {
                card.style.display = 'none';
            }
        });
    });
});

// Star Rating Functionality
const ratingStars = document.querySelectorAll('.v5-rating-input span');
const ratingInput = document.getElementById('ratingInput');

ratingStars.forEach(star => {
    star.addEventListener('click', function() {
        const rating = parseInt(this.getAttribute('data-value'));
        ratingInput.value = rating;
        
        // Update star display
        ratingStars.forEach((s, index) => {
            if (index < rating) {
                s.innerHTML = '<i class="fas fa-star"></i>';
                s.classList.add('active');
            } else {
                s.innerHTML = '<i class="far fa-star"></i>';
                s.classList.remove('active');
            }
        });
    });
    
    // Hover effect
    star.addEventListener('mouseover', function() {
        const hoverRating = parseInt(this.getAttribute('data-value'));
        
        ratingStars.forEach((s, index) => {
            if (index < hoverRating) {
                s.innerHTML = '<i class="fas fa-star"></i>';
            } else {
                s.innerHTML = '<i class="far fa-star"></i>';
            }
        });
    });
    
    star.addEventListener('mouseout', function() {
        const currentRating = parseInt(ratingInput.value);
        
        ratingStars.forEach((s, index) => {
            if (index < currentRating) {
                s.innerHTML = '<i class="fas fa-star"></i>';
            } else {
                s.innerHTML = '<i class="far fa-star"></i>';
            }
        });
    });
});

// Form Submission
document.getElementById('reviewForm').addEventListener('submit', function(e) {
    e.preventDefault();
    
    // Validate rating
    if (ratingInput.value === "0") {
        alert('Please provide a rating by clicking on the stars');
        return;
    }
    
    // Get form values
    const roomType = document.getElementById('reviewRoomType').value;
    const rating = ratingInput.value;
    const title = document.getElementById('reviewTitle').value;
    const review = document.getElementById('reviewText').value;
    const name = document.getElementById('reviewerName').value;
    
    // Create new review card
    const reviewCard = document.createElement('div');
    reviewCard.className = 'v5-review-card';
    reviewCard.setAttribute('data-room', roomType);
    
    // Get initials for avatar
    const initials = name.split(' ').map(n => n[0]).join('').toUpperCase();
    
    // Room type display text
    let roomTypeText = '';
    let roomClass = '';
    switch(roomType) {
        case 'standard':
            roomTypeText = 'Standard Room';
            roomClass = 'standard';
            break;
        case 'deluxe':
            roomTypeText = 'Deluxe Room';
            roomClass = 'deluxe';
            break;
        case 'suite':
            roomTypeText = 'Suite';
            roomClass = 'suite';
            break;
    }
    
    // Current date
    const today = new Date();
    const options = { year: 'numeric', month: 'long', day: 'numeric' };
    const dateString = today.toLocaleDateString('en-US', options);
    
    // Stars HTML
    let starsHtml = '';
    for (let i = 1; i <= 5; i++) {
        starsHtml += `<i class="${i <= rating ? 'fas' : 'far'} fa-star"></i>`;
    }
    
    // Build review card HTML
    reviewCard.innerHTML = `
        <div class="v5-review-header">
            <div class="v5-reviewer-info">
                <div class="v5-reviewer-avatar">${initials}</div>
                <div class="v5-reviewer-details">
                    <h3>${name}</h3>
                    <div class="v5-review-meta">
                        <span class="v5-room-tag ${roomClass}">${roomTypeText}</span>
                        <span class="v5-review-date">${dateString}</span>
                    </div>
                </div>
            </div>
            <div class="v5-review-rating">
                <div class="v5-stars">
                    ${starsHtml}
                </div>
                <span>${rating}/5</span>
            </div>
        </div>
        <div class="v5-review-body">
            <h4>${title}</h4>
            <p>${review}</p>
        </div>
    `;
    
    // Prepend to reviews list
    document.querySelector('.v5-reviews-list').prepend(reviewCard);
    
    // Reset form
    this.reset();
    ratingInput.value = "0";
    ratingStars.forEach(star => {
        star.innerHTML = '<i class="far fa-star"></i>';
        star.classList.remove('active');
    });
    
    // Show success message
    alert('Thank you for your review! It has been added to the list.');
    
    // Filter to show the new review if needed
    const activeFilter = document.querySelector('.v5-filter-option.active').getAttribute('data-room');
    if (activeFilter !== 'all' && activeFilter !== roomType) {
        reviewCard.style.display = 'none';
    }
});