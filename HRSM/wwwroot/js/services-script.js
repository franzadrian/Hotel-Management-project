// Toggle between service type forms
document.querySelectorAll('input[name="serviceType"]').forEach(radio => {
    radio.addEventListener('change', function() {
        // Hide all form sections
        document.querySelectorAll('.v6-dynamic-form [data-service]').forEach(section => {
            section.style.display = 'none';
        });
        
        // Show selected section
        const selectedSection = document.querySelector(`[data-service="${this.value}"]`);
        if (selectedSection) {
            selectedSection.style.display = 'block';
        }
    });
});

// Form submission
document.getElementById('serviceRequestForm').addEventListener('submit', function(e) {
    e.preventDefault();
    
    // Get selected service type
    const serviceType = document.querySelector('input[name="serviceType"]:checked').value;
    
    // Create sample request (in real app this would go to backend)
    const requestItem = document.createElement('div');
    requestItem.className = 'v6-request-item';
    requestItem.innerHTML = `
        <div class="v6-request-meta">
            <span class="v6-badge ${serviceType}">${serviceType.charAt(0).toUpperCase() + serviceType.slice(1)}</span>
            <span class="v6-date">Just now</span>
        </div>
        <p>New ${serviceType} request submitted</p>
        <div class="v6-status pending">Pending</div>
    `;
    
    // Add to top of request list
    document.querySelector('.v6-request-status').insertBefore(
        requestItem, 
        document.querySelector('.v6-request-item')
    );
    
    // Show confirmation
    alert('Your service request has been submitted!');
    
    // Reset form (keep room selection)
    this.reset();
    document.querySelector('[data-service="maintenance"]').style.display = 'block';
});