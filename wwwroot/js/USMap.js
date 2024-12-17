class USMap {
    constructor(section, data) {
        this.section = section; // Section where the map will be populated
        this.data = data; // JSON data containing map details
        this.width = 960;
        this.height = 600;

        // Tooltip configuration
        this.tooltip = null;

        // Projection and Path
        this.projection = d3.geoAlbersUsa()
            .translate([this.width / 2, this.height / 2])
            .scale([1000]);

        this.path = d3.geoPath().projection(this.projection);

        // State color configuration
        this.defaultColor = "#ddd"; // States without data
        this.highlightColor = "#66c2a5"; // States with data

        this.initMap();
    }

    // Initialize the Map
    initMap() {
        // Create the tooltip
        this.tooltip = d3.select("body").append("div")
            .attr("class", "map-tooltip")
            .style("position", "absolute")
            .style("background-color", "white")
            .style("padding", "8px")
            .style("border", "1px solid #ddd")
            .style("border-radius", "5px")
            .style("box-shadow", "0 0 5px rgba(0,0,0,0.3)")
            .style("pointer-events", "none")
            .style("display", "none");

        // Create the SVG container
        this.svg = d3.select(this.section)
            .append("svg")
            .attr("width", this.width)
            .attr("height", this.height);

        // Load and render the map
        this.loadMapData();
    }

    loadMapData() {
        // Load TopoJSON for U.S. states
        d3.json("https://cdn.jsdelivr.net/npm/us-atlas@3/states-10m.json").then((us) => {
            const states = topojson.feature(us, us.objects.states);

            // Create a data map for easy access
            const stateDataMap = new Map(this.data.mapData.map(d => [d.state, d]));

            // Draw States
            this.svg.selectAll(".state")
                .data(states.features)
                .enter()
                .append("path")
                .attr("class", "state")
                .attr("d", this.path)
                .attr("fill", d => {
                    const stateName = d.properties.name;
                    const stateData = stateDataMap.get(stateName);
                    return stateData ? this.highlightColor : this.defaultColor;
                })
                .attr("stroke", "#fff")
                .attr("stroke-width", 1)
                .on("mouseover", (event, d) => {
                    const stateName = d.properties.name;
                    const stateData = stateDataMap.get(stateName);

                    if (stateData) {
                        this.tooltip.style("display", "block")
                            .html(`
                  <strong>${stateName}</strong><br>
                  Total URLs: ${stateData.totalUrls}<br>
                  Errors: ${stateData.errors}<br>
                  Offline: ${stateData.offline}<br>
                  Online: ${stateData.online}
                `)
                            .style("left", (event.pageX + 10) + "px")
                            .style("top", (event.pageY - 20) + "px");
                    }
                })
                .on("mouseout", () => {
                    this.tooltip.style("display", "none");
                });
        });
    }
}

// Expose the renderUSMap function globally
window.renderUSMap = function (sectionId, jsonData) {
    console.log("Calling renderUSMap with:", sectionId, jsonData);
    new USMap(sectionId, jsonData);
};